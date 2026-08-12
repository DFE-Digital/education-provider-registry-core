using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;

/// <summary>
/// Defines a component responsible for constructing the base
/// <see cref="IQueryable{TProjection}"/> used by the search pipeline.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type produced by the query.
/// </typeparam>
public interface ISearchProjectionBuilder<TProjection>
{
    /// <summary>
    /// Builds an EF‑translatable query for <typeparamref name="TProjection"/>
    /// from the supplied <see cref="DbContext"/>.
    /// </summary>
    /// <param name="db">The active EF Core context.</param>
    /// <returns>An unexecuted <see cref="IQueryable{TProjection}"/>.</returns>
    IQueryable<TProjection> Build(DbContext db);
}
