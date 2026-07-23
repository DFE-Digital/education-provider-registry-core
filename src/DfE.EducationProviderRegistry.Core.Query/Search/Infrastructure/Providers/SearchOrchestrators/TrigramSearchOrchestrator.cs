using System.Reflection;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

internal sealed class TrigramSearchOrchestrator<TProjection> : ISearchOrchestrator<TProjection>
    where TProjection : class
{
    private readonly IEntityMetadataResolver<TProjection> _metadataResolver;
    private readonly ISqlExecutor<TProjection> _sqlExecutor;

    public TrigramSearchOrchestrator(
        IEntityMetadataResolver<TProjection> metadataResolver,
        ISqlExecutor<TProjection> sqlExecutor)
    {
        _metadataResolver = metadataResolver
            ?? throw new ArgumentNullException(nameof(metadataResolver));

        _sqlExecutor = sqlExecutor
            ?? throw new ArgumentNullException(nameof(sqlExecutor));
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

        if (context.SearchColumnConfig is null || !context.SearchColumnConfig.Any())
            throw new ArgumentException("At least one search column must be provided");

        EntityMetadata metadata = _metadataResolver.Resolve(db);

        // TODO: we could move all of this
        const string SearchParameter = "@searchTerm";
        const string pageSizeParameter = "@pageSize";
        const string offsetParameter = "@offset";

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

        // Biild sql dynamically based on each columns config strategy
        foreach (SearchColumnConfig colConfig in context.SearchColumnConfig)
        {
            string tableAlias = "t";

            // resolve target entity and column name
            IEntityType targetEntityType = metadata.EntityType;

            // TODO: we culd move all of this, expression building
            if (!string.IsNullOrWhiteSpace(colConfig.NavigationProperty))
            {
                tableAlias = $"j_{colConfig.NavigationProperty.ToLowerInvariant()}";

                if (!joins.ContainsKey(tableAlias))
                {
                    INavigation navigation = metadata.EntityType.FindNavigation(colConfig.NavigationProperty)
                        ?? throw new InvalidOperationException($"Navigation property '{colConfig.NavigationProperty}' not found on {metadata.EntityType.Name}");

                    targetEntityType = navigation.TargetEntityType;
                    IForeignKey fk = navigation.ForeignKey;

                    string targetTable = targetEntityType.GetTableName()!;
                    string targetSchema = targetEntityType.GetSchema() ?? metadata.Schema;

                    // build join condition based on foreign key direction
                    string joinCondition;
                    if (fk.DeclaringEntityType == metadata.EntityType)
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

                    joins[tableAlias] = $"LEFT JOIN {targetSchema}.\"{targetTable}\" {tableAlias} ON {joinCondition}";
                }
                else
                {
                    INavigation navigation = metadata.EntityType.FindNavigation(colConfig.NavigationProperty)!;
                    targetEntityType = navigation.TargetEntityType;
                }
            }

            // validate column exists on target entity
            // TODO: We could move this
            IProperty targetProperty = targetEntityType.GetProperties()
                .FirstOrDefault(p => p.GetColumnName().Equals(colConfig.ColumnName, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException(
                    $"Column '{colConfig.ColumnName}' does not existon target entity '{targetEntityType.Name}'");

            string targetColumn = targetProperty.GetColumnName();

            // build sql conditions for this column
            // TODO: We could move this
            switch (colConfig.Strategy)
            {
                case MatchStrategy.ExactPartialFuzzy:
                    whereConditions.Add(
                        $"(LOWER({tableAlias}.\"{targetColumn}\") = LOWER({SearchParameter}) OR " +
                        $"{tableAlias}.\"{targetColumn}\" ILIKE '%' || {SearchParameter} || '%' OR " +
                        $"{tableAlias}.\"{targetColumn}\" % {SearchParameter})");

                    tierExpressions.Add($"" +
                        $"CASE " +
                            $"WHEN LOWER({tableAlias}.\"{targetColumn}\") = LOWER({SearchParameter}) THEN 3 " +
                            $"WHEN {tableAlias}.\"{targetColumn}\" ILIKE '%' || {SearchParameter} || '%' THEN 2 " +
                            $"WHEN {tableAlias}.\"{targetColumn}\" % {SearchParameter} THEN 1 " +
                            $"ELSE 0 " +
                        $"END");

                    similarityExpressions.Add($"similarity({tableAlias}.\"{targetColumn}\", {SearchParameter})");
                    break;
                case MatchStrategy.ExactPartial:
                    whereConditions.Add(
                        $"(LOWER({tableAlias}.\"{targetColumn}\") = LOWER({SearchParameter}) OR " +
                        $"{tableAlias}.\"{targetColumn}\" ILIKE '%' || {SearchParameter} || '%')");

                    tierExpressions.Add($"" +
                        $"CASE " +
                            $"WHEN LOWER({tableAlias}.\"{targetColumn}\") = LOWER({SearchParameter}) THEN 3 " +
                            $"WHEN {tableAlias}.\"{targetColumn}\" ILIKE '%' || {SearchParameter} || '%' THEN 2 " +
                            $"ELSE 0 " +
                        $"END");

                    similarityExpressions.Add("0.0");
                    break;
                default:
                    break;
            }
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
        ),
        ranked_matches AS (
            SELECT
                *,
                MAX(match_score) OVER () AS best_overall_score
            FROM scored_matches
            WHERE match_score > 0
        )
        SELECT ""{metadata.PrimaryKeyColumn}""
        FROM ranked_matches
        WHERE
            (best_overall_score = 3 AND match_score = 3)
            OR
            (best_overall_score < 3 AND match_score >= 1)
        ORDER BY
            match_score DESC,
            similarity_score DESC
        LIMIT {pageSizeParameter}
        OFFSET {offsetParameter}
        ";



        List<object> ids = await _sqlExecutor.ExecuteIdsAsync(
            db,
            sql,
            metadata.PrimaryKeyProperty.Name,
            parameters,
            cancellationToken);

        return [
            .. baseQuery
                .AsEnumerable()
                .Where(projection =>
                    ids.Contains(GetPrimaryKeyValue(
                        projection, metadata.PrimaryKeyProperty.Name)))
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



public enum MatchStrategy
{
    ExactPartial, // e.g. URN, Postcode, Identifiers
    ExactPartialFuzzy // e.g. Name, Town, City,
}

public record SearchColumnConfig(
    string ColumnName,
    MatchStrategy Strategy = MatchStrategy.ExactPartialFuzzy,
    string? NavigationProperty = null);
