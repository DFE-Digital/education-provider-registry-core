using System.Reflection;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram;

/// <summary>
/// Executes a PostgreSQL trigram similarity search using <c>pg_trgm</c> operators,
/// retrieves matching primary keys via raw SQL, and rehydrates full
/// <typeparamref name="TProjection"/> instances using EF Core LINQ.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type being queried.
/// </typeparam>
public sealed class TrigramSearchOrchestrator<TProjection> : ISearchOrchestrator<TProjection>
    where TProjection : class
{
    private readonly IEntityMetadataResolver<TProjection> _metadataResolver;
    private readonly ISqlFilterExpressionTranslator<TProjection> _sqlFilterExpressionTranslator;
    private readonly ISqlExecutor<TProjection> _sqlExecutor;

    /// <summary>
    /// Creates a new trigram search orchestrator.
    /// </summary>
    /// <param name="metadataResolver">Resolves EF Core metadata for the projection type.</param>
    /// <param name="sqlFilterExpressionTranslator">Translates LINQ filter expressions into SQL.</param>
    /// <param name="sqlExecutor">Executes raw SQL and returns primary key values.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any dependency is <c>null</c>.
    /// </exception>
    public TrigramSearchOrchestrator(
        IEntityMetadataResolver<TProjection> metadataResolver,
        ISqlFilterExpressionTranslator<TProjection> sqlFilterExpressionTranslator,
        ISqlExecutor<TProjection> sqlExecutor)
    {
        _metadataResolver = metadataResolver
            ?? throw new ArgumentNullException(nameof(metadataResolver));
        _sqlFilterExpressionTranslator = sqlFilterExpressionTranslator
            ?? throw new ArgumentNullException(nameof(sqlFilterExpressionTranslator));
        _sqlExecutor = sqlExecutor
            ?? throw new ArgumentNullException(nameof(sqlExecutor));
    }

    /// <summary>
    /// Executes a trigram similarity search against the specified column, applies
    /// filter expressions, retrieves matching primary keys, and rehydrates full
    /// <typeparamref name="TProjection"/> instances.
    /// </summary>
    /// <param name="dbContext">The EF Core context used for metadata and SQL execution.</param>
    /// <param name="baseQuery">The base LINQ query used to rehydrate results.</param>
    /// <param name="context">Search configuration including term, column, paging, and filters.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A read‑only list of matching projections.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dbContext"/> or <paramref name="context"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the search column does not exist on the entity.
    /// </exception>
    public async Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext dbContext,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext<TProjection> context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(context);

        EntityMetadata metadata = _metadataResolver.Resolve(dbContext);

        bool columnExists =
            metadata.EntityType
                .GetProperties()
                .Any(property =>
                    property.GetColumnName() == context.SearchColumn);

        if (!columnExists)
        {
            throw new InvalidOperationException(
                $"Column '{context.SearchColumn}' does not exist on entity {typeof(TProjection).Name}.");
        }

        string sql =
            $@"
            SELECT t.""{metadata.PrimaryKeyColumn}""
            FROM {metadata.Schema}.""{metadata.TableName}"" t
            WHERE t.""{context.SearchColumn}"" % CAST('{context.SearchTerm}' AS text)
            AND {_sqlFilterExpressionTranslator.Translate(context.FilterExpression, metadata)}
            ORDER BY similarity(t.""{context.SearchColumn}"", CAST('{context.SearchTerm}' AS text)) DESC
            LIMIT {context.PageSize} OFFSET {context.Offset}";

        List<object> ids =
            await _sqlExecutor.ExecuteIdsAsync(
                dbContext,
                sql,
                metadata.PrimaryKeyProperty.Name,
                cancellationToken);

        IQueryable<TProjection> filteredQuery =
            baseQuery.Where(context.FilterExpression);

        List<TProjection> results =
            [.. filteredQuery
                .AsEnumerable()
                .Where(projection =>
                    ids.Contains(GetPrimaryKeyValue(
                        projection,
                        metadata.PrimaryKeyProperty.Name)))];

        return results.AsReadOnly();
    }

    /// <summary>
    /// Extracts the primary key value from a projection instance using reflection.
    /// </summary>
    /// <param name="entity">The projection instance.</param>
    /// <param name="pkName">The CLR name of the primary key property.</param>
    /// <returns>The primary key value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the entity is <c>null</c>, the property does not exist,
    /// or the primary key value is <c>null</c>.
    /// </exception>
    private static object GetPrimaryKeyValue(TProjection entity, string pkName)
    {
        if (entity is null)
        {
            throw new InvalidOperationException(
                $"Entity instance is null when evaluating primary key '{pkName}'.");
        }

        PropertyInfo pkProp =
            entity.GetType().GetProperty(pkName)
            ?? throw new InvalidOperationException(
                $"Primary key property '{pkName}' not found on type '{entity.GetType().Name}'.");

        object? value = pkProp.GetValue(entity);

        return value is null
            ? throw new InvalidOperationException(
                $"Primary key value for '{pkName}' is null on entity '{entity.GetType().Name}'.")
            : value;
    }
}
