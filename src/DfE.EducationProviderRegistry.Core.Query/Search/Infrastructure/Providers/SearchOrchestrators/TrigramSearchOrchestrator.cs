using System.Reflection;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Test;
using Microsoft.EntityFrameworkCore;

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

        EntityMetadata metadata = _metadataResolver.Resolve(db);

        List<string> invalidColumns = context.SearchColumns
            .Where(searchColumn =>
                !metadata.EntityType
                    .GetProperties()
                    .Any(property => property.GetColumnName() == searchColumn))
            .ToList();

        if (invalidColumns.Count is not 0)
        {
            throw new InvalidOperationException(
                $"Column(s) '{string.Join("', '", invalidColumns)}' do not exist on entity {typeof(TProjection).Name}.");
        }


        bool isNumericSearch = context.SearchTerm.All(char.IsDigit);

        string sql;

        if (isNumericSearch)
        {
            string exactMatchClause = string.Join(
                " OR ",
                context.SearchColumns.Select(column =>
                    $@"t.""{column}"" = CAST('{context.SearchTerm}' AS text)"));

            string partialMatchClause = string.Join(
                " OR ",
                context.SearchColumns.Select(column =>
                    $@"t.""{column}"" LIKE CAST('{context.SearchTerm}' AS text) || '%'"));

            sql =
                $@"
                SELECT t.""{metadata.PrimaryKeyColumn}""
                FROM {metadata.Schema}.""{metadata.TableName}"" t
                WHERE (
                    {exactMatchClause}
                    OR {partialMatchClause}
                )
                {searchFilters}
                ORDER BY
                    CASE
                        WHEN ({exactMatchClause}) THEN 1
                        ELSE 2
                    END,
                    t.""{context.SearchColumns.First()}"" ASC
                LIMIT {context.PageSize} OFFSET {context.Offset}
                ";
        }
        else
        {
            string exactMatchClause = string.Join(
                " OR ",
                context.SearchColumns.Select(column =>
                    $@"t.""{column}"" = CAST('{context.SearchTerm}' AS text)"));

            string partialMatchClause = string.Join(
                " OR ",
                context.SearchColumns.Select(column =>
                    $@"t.""{column}"" ILIKE '%' || CAST('{context.SearchTerm}' AS text) || '%'"));

            string fuzzyMatchClause = string.Join(
                " OR ",
                context.SearchColumns.Select(column =>
                    $@"t.""{column}"" % CAST('{context.SearchTerm}' AS text)"));

            string similarityClause = string.Join(
                ", ",
                context.SearchColumns.Select(column =>
                    $@"similarity(t.""{column}"", CAST('{context.SearchTerm}' AS text))"));

            sql =
                $@"
                SELECT t.""{metadata.PrimaryKeyColumn}""
                FROM {metadata.Schema}.""{metadata.TableName}"" t
                WHERE (
                    {exactMatchClause}
                    OR {partialMatchClause}
                    OR {fuzzyMatchClause}
                )
                {searchFilters}
                ORDER BY
                    CASE
                        WHEN ({exactMatchClause}) THEN 1
                        WHEN ({partialMatchClause}) THEN 2
                        ELSE 3
                    END,
                    GREATEST({similarityClause}) DESC
                LIMIT {context.PageSize} OFFSET {context.Offset}
                ";
        }

        List<object> ids = await _sqlExecutor.ExecuteIdsAsync(
            db,
            sql,
            metadata.PrimaryKeyProperty.Name,
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

internal sealed class SearchOrchestrator<TProjection> : ISearchOrchestrator<TProjection>
    where TProjection : class
{
    private readonly IEntityMetadataResolver<TProjection> _metadataResolver;
    private readonly ISqlExecutor<TProjection> _sqlExecutor;
    private readonly ISearchQueryDefinitionFactory _definitionFactory;
    private readonly ISearchQueryBuilder _queryBuilder;
    private readonly ISearchColumnValidator _columnValidator;

    public SearchOrchestrator(
        IEntityMetadataResolver<TProjection> metadataResolver,
        ISqlExecutor<TProjection> sqlExecutor,
        ISearchQueryDefinitionFactory definitionFactory,
        ISearchQueryBuilder queryBuilder,
        ISearchColumnValidator columnValidator)
    {
        _metadataResolver = metadataResolver;
        _sqlExecutor = sqlExecutor;
        _definitionFactory = definitionFactory;
        _queryBuilder = queryBuilder;
        _columnValidator = columnValidator;
    }

    public async Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext db,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext context,
        string searchFilters = "",
        CancellationToken cancellationToken = default)
    {
        EntityMetadata metadata =
            _metadataResolver.Resolve(db);

        _columnValidator.Validate<TProjection>(
            metadata,
            context);

        SearchMode mode =
            context.SearchTerm.All(char.IsDigit)
                ? SearchMode.Numeric
                : SearchMode.Text;

        SearchQueryDefinition definition =
            _definitionFactory.Create(
                mode,
                context);

        string sql =
            _queryBuilder.Build(
                metadata,
                context,
                definition,
                searchFilters);

        List<object> ids =
            await _sqlExecutor.ExecuteIdsAsync(
                db,
                sql,
                metadata.PrimaryKeyProperty.Name,
                cancellationToken);

        return
        [
            .. baseQuery
                .AsEnumerable()
                .Where(x =>
                    ids.Contains(
                        GetPrimaryKeyValue(
                            x,
                            metadata.PrimaryKeyProperty.Name)))
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
