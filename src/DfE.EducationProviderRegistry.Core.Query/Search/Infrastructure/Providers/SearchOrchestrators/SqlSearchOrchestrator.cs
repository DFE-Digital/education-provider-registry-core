using System.Reflection;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

public enum MatchStrategy
{
    ExactPartial, // e.g. URN, Postcode, Identifiers
    ExactPartialFuzzy // e.g. Name, Town, City,
}

public record SearchColumnConfig(
    string ColumnName,
    MatchStrategy Strategy = MatchStrategy.ExactPartialFuzzy,
    string? NavigationProperty = null);

public class EstablishmentSearchOptions
{
    public const string SectionName = "EstablishmentSearchOptions";

    public List<SearchColumnConfig> SearchColumns { get; set; } = new()
    {
        new(ColumnName: "urn", Strategy: MatchStrategy.ExactPartial),
        new(ColumnName: "name", Strategy: MatchStrategy.ExactPartialFuzzy),
        new(ColumnName: "postcode", Strategy: MatchStrategy.ExactPartial, NavigationProperty: "Site"),
        //new(ColumnName: "group_id", Strategy: MatchStrategy.ExactPartial, NavigationProperty: "EstablishmentGroupMembership")
    };
}



public record MatchStrategyResult(
    string WhereCondition,
    string TierExpression,
    string SimilarityExpression);

public interface IMatchStrategyHandler
{
    bool CanHandle(MatchStrategy strategy);
    MatchStrategyResult BuildExpressions(string tableAlias, string targetColumn, string searchParameter);
}

public sealed class SqlExactPartialMatchStrategyHandler : IMatchStrategyHandler
{
    public bool CanHandle(MatchStrategy strategy) => strategy == MatchStrategy.ExactPartial;

    public MatchStrategyResult BuildExpressions(string tableAlias, string targetColumn, string searchParameter)
    {
        string columnRef = $"{tableAlias}.\"{targetColumn}\"";

        string whereCondition = $"({columnRef} ILIKE '%' || {searchParameter} || '%' " +
            $"OR LOWER({columnRef}) = LOWER({searchParameter}))";

        string tierExpression = $@"CASE
            WHEN LOWER({columnRef}) = LOWER({searchParameter}) THEN 4
            WHEN {columnRef} ILIKE {searchParameter} || '%' THEN 3
            WHEN {columnRef} ILIKE '%' || {searchParameter} || '%' THEN 2
            ELSE 0
        END";

        string similiarityExpression = "0.0";

        return new MatchStrategyResult(whereCondition, tierExpression, similiarityExpression);
    }
}

public sealed class SqlExactPartialFuzzyMatchStrategyHandler : IMatchStrategyHandler
{
    public bool CanHandle(MatchStrategy strategy) => strategy == MatchStrategy.ExactPartialFuzzy;

    public MatchStrategyResult BuildExpressions(string tableAlias, string targetColumn, string searchParameter)
    {
        string columnRef = $"{tableAlias}.\"{targetColumn}\"";

        string whereCondition = $"({columnRef} ILIKE '%' || {searchParameter} || '%' " +
            $"OR LOWER({columnRef}) = LOWER({searchParameter}) " +
            $"OR {columnRef} % {searchParameter})";

        string tierExpression = $@"CASE
            WHEN LOWER({columnRef}) = LOWER({searchParameter}) THEN 4
            WHEN {columnRef} ILIKE {searchParameter} || '%' THEN 3
            WHEN {columnRef} ILIKE '%' || {searchParameter} || '%' THEN 2
            WHEN {columnRef} % {searchParameter} THEN 1
            ELSE 0
        END";

        string similiarityExpression = $"similarity({columnRef}, {searchParameter})";

        return new MatchStrategyResult(whereCondition, tierExpression, similiarityExpression);
    }
}



public record JoinResolutionResult(
    string TableAlias,
    IEntityType TargetEntityType,
    string? JoinClause);

public interface IJoinClauseBuilder
{
    JoinResolutionResult ResolveJoin(
        EntityMetadata rootMetadata,
        string navigationProperty,
        IDictionary<string, string> existingJoins);
}

public sealed class JoinClauseBuilder : IJoinClauseBuilder
{
    public JoinResolutionResult ResolveJoin(EntityMetadata rootMetadata, string navigationProperty, IDictionary<string, string> existingJoins)
    {
        string tableAlias = $"j_{navigationProperty.ToLowerInvariant()}";

        INavigation navigation = rootMetadata.EntityType.FindNavigation(navigationProperty)
            ?? throw new InvalidOperationException($"Navigation propert '{navigationProperty} not found on {rootMetadata.EntityType.Name}");

        IEntityType targetEntityType = navigation.TargetEntityType;
        if (existingJoins.ContainsKey(tableAlias))
            return new JoinResolutionResult(tableAlias, targetEntityType, null);

        IForeignKey fk = navigation.ForeignKey;
        string targetTable = targetEntityType.GetTableName()!;
        string targetSchema = targetEntityType.GetSchema() ?? rootMetadata.Schema;

        string joinCondition;
        if (fk.DeclaringEntityType == rootMetadata.EntityType)
        {
            string fkCol = fk.Properties[0].GetColumnName();
            string pkCol = fk.PrincipalKey.Properties[0].GetColumnName();
            joinCondition = $"t.\"{fkCol}\" = {tableAlias}.\"{pkCol}\"";
        }
        else
        {
            string fkCol = fk.Properties[0].GetColumnName();
            string pkCol = fk.PrincipalKey.Properties[0].GetColumnName();
            joinCondition = $"{tableAlias}.\"{fkCol}\" = t.\"{pkCol}\"";
        }

        string joinClause = $"LEFT JOIN {targetSchema}.\"{targetTable}\" {tableAlias} ON {joinCondition}";
        return new JoinResolutionResult(tableAlias, targetEntityType, joinClause);
    }
}




public record BuiltSearchQuery(
    string Sql,
    object[] Parameters);

public interface ISearchQueryBuilder
{
    BuiltSearchQuery BuildQuery(
        EntityMetadata metadata,
        SearchOrchestratorContext context,
        string searchFilters);
}

public sealed class SqlSearchQueryBuilder : ISearchQueryBuilder
{
    private const string SearchParameter = "@searchTerm";
    private const string PageSizeParameter = "@pageSize";
    private const string OffsetParameter = "@offset";

    private readonly IJoinClauseBuilder _joinClauseBuilder;
    private readonly IEnumerable<IMatchStrategyHandler> _strategyHandlers;

    public SqlSearchQueryBuilder(
        IJoinClauseBuilder joinClauseBuilder,
        IEnumerable<IMatchStrategyHandler> strategyHandlers)
    {
        _joinClauseBuilder = joinClauseBuilder;
        _strategyHandlers = strategyHandlers;
    }

    public BuiltSearchQuery BuildQuery(EntityMetadata metadata, SearchOrchestratorContext context, string searchFilters)
    {
        if (context.SearchColumnConfig is null || !context.SearchColumnConfig.Any())
            throw new ArgumentException("At least one search column configuration must be provided");

        object[] parameters =
        [
            new NpgsqlParameter("searchTerm", context.SearchTerm),
            new NpgsqlParameter<int>("pageSize", context.PageSize),
            new NpgsqlParameter<int>("offset", context.Offset)
        ];

        Dictionary<string, string> joins = []; // Alias -> Join clause
        List<string> whereConditions = [];
        List<string> tierExpressions = [];
        List<string> similarityExpressions = [];

        foreach (var colConfig in context.SearchColumnConfig)
        {
            string tableAlias = "t";
            IEntityType targetEntityType = metadata.EntityType;

            if (!string.IsNullOrWhiteSpace(colConfig.NavigationProperty))
            {
                var joinResult = _joinClauseBuilder.ResolveJoin(metadata, colConfig.NavigationProperty, joins);
                tableAlias = joinResult.TableAlias;
                targetEntityType = joinResult.TargetEntityType;

                if (joinResult.JoinClause is not null)
                    joins[tableAlias] = joinResult.JoinClause;
            }

            IProperty targetProperty = targetEntityType.GetProperties()
                .FirstOrDefault(p => p.GetColumnName().Equals(colConfig.ColumnName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Column '{colConfig.ColumnName}' does not exist on entity '{targetEntityType.Name}'.");

            string targetColumn = targetProperty.GetColumnName();

            IMatchStrategyHandler matchStrategyHandler = _strategyHandlers.FirstOrDefault(h => h.CanHandle(colConfig.Strategy))
                ?? throw new InvalidOperationException($"Match strategy '{colConfig.Strategy}' is not supported");

            MatchStrategyResult matchResult = matchStrategyHandler.BuildExpressions(tableAlias, targetColumn, SearchParameter);

            whereConditions.Add(matchResult.WhereCondition);
            tierExpressions.Add(matchResult.TierExpression);
            similarityExpressions.Add(matchResult.SimilarityExpression);
        }

        string joinStatements = string.Join("\n", joins.Values);
        string combinedWhere = string.Join(" OR ", whereConditions);

        string matchingTierExpression = tierExpressions.Count > 1
            ? $"GREATEST({string.Join(", ", tierExpressions)})"
            : tierExpressions.First();

        string similarityExpression = similarityExpressions.Count > 1
            ? $"GREATEST({string.Join(", ", similarityExpressions)})"
            : similarityExpressions.First();

        string sql =
       $@"
        WITH row_scores AS (
            SELECT
                t.""{metadata.PrimaryKeyColumn}"",
                {matchingTierExpression} AS match_score,
                {similarityExpression} AS similarity_score
            FROM {metadata.Schema}.""{metadata.TableName}"" t
            {joinStatements}
            WHERE ({combinedWhere})
            {searchFilters}
        ),
        scored_matches AS (
            SELECT
                ""{metadata.PrimaryKeyColumn}"",
                MAX(match_score) AS match_score,
                MAX(similarity_score) AS similarity_score
            FROM row_scores
            GROUP BY ""{metadata.PrimaryKeyColumn}""
        )
        SELECT ""{metadata.PrimaryKeyColumn}""
        FROM scored_matches
        WHERE match_score > 0
        ORDER BY
            match_score DESC,
            similarity_score DESC
        LIMIT {PageSizeParameter}
        OFFSET {OffsetParameter}
        ";

        return new BuiltSearchQuery(sql, parameters);
    }
}




internal sealed class SqlSearchOrchestrator<TProjection> : ISearchOrchestrator<TProjection>
    where TProjection : class
{
    private readonly IEntityMetadataResolver<TProjection> _metadataResolver;
    private readonly ISqlExecutor<TProjection> _sqlExecutor;
    private readonly ISearchQueryBuilder _queryBuilder;

    public SqlSearchOrchestrator(
        IEntityMetadataResolver<TProjection> metadataResolver,
        ISqlExecutor<TProjection> sqlExecutor,
        ISearchQueryBuilder searchQueryBuilder)
    {
        _metadataResolver = metadataResolver
            ?? throw new ArgumentNullException(nameof(metadataResolver));

        _sqlExecutor = sqlExecutor
            ?? throw new ArgumentNullException(nameof(sqlExecutor));

        _queryBuilder = searchQueryBuilder;
    }

    public async Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext db,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext context,
        string searchFilters = "",
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(context);

        EntityMetadata metadata = _metadataResolver.Resolve(db);

        BuiltSearchQuery sqlQuery = _queryBuilder.BuildQuery(metadata, context, searchFilters);

        List<object> ids = await _sqlExecutor.ExecuteIdsAsync(
            db,
            sqlQuery.Sql,
            metadata.PrimaryKeyProperty.Name,
            sqlQuery.Parameters,
            cancellationToken);

        return [
            .. baseQuery
                .AsEnumerable()
                .Where(projection => ids.Contains(GetPrimaryKeyValue(projection, metadata.PrimaryKeyProperty.Name)))
            ];
    }

    private static object GetPrimaryKeyValue(TProjection entity, string pkName)
    {
        if (entity is null)
            throw new InvalidOperationException(
                $"Entity instance is null when evaluating primary key '{pkName}'.");

        PropertyInfo pkProp = entity.GetType().GetProperty(pkName)
            ?? throw new InvalidOperationException(
                $"Primary key property '{pkName}' not found on type '{entity.GetType().Name}'.");

        return pkProp.GetValue(entity)
            ?? throw new InvalidOperationException(
                $"Primary key value for '{pkName}' is null on entity '{entity.GetType().Name}'.");
    }
}
