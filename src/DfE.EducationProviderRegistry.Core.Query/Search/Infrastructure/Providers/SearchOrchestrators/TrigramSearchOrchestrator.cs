using System.Reflection;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
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

        if (!metadata.EntityType
            .GetProperties()
                .Any(property =>
                    property.GetColumnName() == context.SearchColumn))
        {
            throw new InvalidOperationException(
                $"Column '{context.SearchColumn}' does not exist on entity {typeof(TProjection).Name}.");
        }

        // Build trigram SQL
        string searchTerm = context.SearchTerm.Replace("'", "''");
        string sql =
            $@"
            SELECT t.""{metadata.PrimaryKeyColumn}""
            FROM {metadata.Schema}.""{metadata.TableName}"" t
            WHERE t.""{context.SearchColumn}"" % CAST('{searchTerm}' AS text)
            {searchFilters}
            ORDER BY similarity(t.""{context.SearchColumn}"", CAST('{searchTerm}' AS text)) DESC
            LIMIT {context.PageSize} OFFSET {context.Offset}
            ";

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
