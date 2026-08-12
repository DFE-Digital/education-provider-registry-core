using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

/// <summary>
/// Defines a component capable of resolving EF Core metadata for a given
/// projection or entity type.
/// </summary>
/// <typeparam name="TProjection">
/// The EF‑mapped entity or projection type whose metadata should be resolved.
/// </typeparam>
public interface IEntityMetadataResolver<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Resolves EF Core metadata for <typeparamref name="TProjection"/> using
    /// the supplied <see cref="DbContext"/> instance.
    /// </summary>
    /// <param name="db">The EF Core <see cref="DbContext"/> whose model is inspected.</param>
    /// <returns>
    /// A populated <see cref="EntityMetadata"/> describing schema, table name,
    /// primary key property, and primary key column mappings.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> is null.
    /// </exception>
    EntityMetadata Resolve(DbContext db);
}
