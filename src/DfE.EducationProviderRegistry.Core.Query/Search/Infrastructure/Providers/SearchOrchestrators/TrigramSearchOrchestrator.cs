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

        Dictionary<string, IProperty> entityProperties = metadata.EntityType
            .GetProperties()
            .ToDictionary(property => property.GetColumnName(), property => property, StringComparer.OrdinalIgnoreCase);

        // validate target columns against EF core metadata
        foreach (SearchColumnConfig colConfig in context.SearchColumnConfig)
        {
            if (!entityProperties.ContainsKey(colConfig.ColumnName))
            {
                throw new InvalidOperationException(
                    $"Column '{context.SearchColumn}' does not exist on entity {typeof(TProjection).Name}.");
            }
        }

        object[] parameters =
        [
            new NpgsqlParameter("searchTerm", context.SearchTerm),
            new NpgsqlParameter<int>("pageSize", context.PageSize),
            new NpgsqlParameter<int>("offset", context.Offset)
        ];

        const string SearchParameter = "@searchTerm";
        const string pageSizeParameter = "@pageSize";
        const string offsetParameter = "@offset";

        List<string> whereConditions = new();
        List<string> tierExpressions = new();
        List<string> similarityExpressions = new();

        // Biild sql dynamically based on each columns config strategy
        foreach (SearchColumnConfig colConfig in context.SearchColumnConfig)
        {
            string col = colConfig.ColumnName;
            switch (colConfig.Strategy)
            {
                case MatchStrategy.ExactPartialFuzzy:
                    whereConditions.Add(
                        $"(LOWER(t.\"{col}\") = LOWER({SearchParameter}) OR " +
                        $"t.\"{col}\" ILIKE '%' || {SearchParameter} || '%' OR " +
                        $"t.\"{col}\" % {SearchParameter})");

                    tierExpressions.Add($"" +
                        $"CASE " +
                            $"WHEN LOWER(t.\"{col}\") = LOWER({SearchParameter}) THEN 3 " +
                            $"WHEN t.\"{col}\" ILIKE '%' || {SearchParameter} || '%' THEN 2 " +
                            $"WHEN t.\"{col}\" % {SearchParameter} THEN 1 " +
                            $"ELSE 0 " +
                        $"END");

                    similarityExpressions.Add($"similarity(t.\"{col}\", {SearchParameter})");
                    break;
                case MatchStrategy.ExactPartial:
                    whereConditions.Add(
                        $"(LOWER(t.\"{col}\") = LOWER({SearchParameter}) OR " +
                        $"t.\"{col}\" ILIKE '%' || {SearchParameter} || '%')");

                    tierExpressions.Add($"" +
                        $"CASE " +
                            $"WHEN LOWER(t.\"{col}\") = LOWER({SearchParameter}) THEN 3 " +
                            $"WHEN t.\"{col}\" ILIKE '%' || {SearchParameter} || '%' THEN 2 " +
                            $"ELSE 0 " +
                        $"END");

                    similarityExpressions.Add("0.0");
                    break;
                default:
                    break;
            }
        }

        string combinedWhere = string.Join(" OR ", whereConditions);
        string matchingTierExpression = tierExpressions.Count > 1
            ? $"GREATEST({string.Join(", ", tierExpressions)})"
            : tierExpressions.First();

        string similarityExpression = similarityExpressions.Count > 1
            ? $"GREATEST({string.Join(", ", similarityExpressions)})"
            : similarityExpressions.First();


        // CTE query executing exact-match cutoff and ordering
        string sql =
           $@"
            WITH scored_matches AS (
                SELECT
                    t.""{metadata.PrimaryKeyProperty.GetColumnName()}"",
                    {matchingTierExpression} AS match_score,
                    {similarityExpression} AS similarity_score
                FROM {metadata.Schema}.""{metadata.TableName}"" t
                WHERE ({combinedWhere})
                {searchFilters}
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
    MatchStrategy Strategy = MatchStrategy.ExactPartialFuzzy);
