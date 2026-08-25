using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;

/// <summary>
/// Builds the base <see cref="IQueryable{T}"/> used for establishment search
/// by applying all required includes for related entities.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class EstablishmentSearchProjectionBuilder
    : ISearchProjectionBuilder<Establishment>
{
    /// <summary>
    /// Produces an <see cref="IQueryable{Establishment}"/> with all navigation
    /// properties required for search projection eagerly loaded.
    /// </summary>
    /// <param name="db">The active EF Core <see cref="DbContext"/>.</param>
    /// <returns>An <see cref="IQueryable{Establishment}"/> with required includes.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> is null.
    /// </exception>
    public IQueryable<Establishment> Build(DbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        EducationProviderRegistryDbContext ctx =
            (EducationProviderRegistryDbContext)db;

        return ctx.Establishment
            .AsNoTracking()
            .AsSplitQuery()
            .Include(establishemnt => establishemnt.Site)
            .Include(establishemnt => establishemnt.EstablishmentType)
            .Include(establishemnt => establishemnt.EstablishmentAuthority)
            .Include(establishemnt => establishemnt.EstablishmentGroupMembership)
                .ThenInclude(groupMembership => groupMembership.Group)
                    .ThenInclude(group => group.GroupType);
    }
}
