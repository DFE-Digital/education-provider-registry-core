using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Provides facet aggregation for <see cref="Establishment"/> entities by grouping
/// a filtered set of URNs on a specified facet selector and returning bucket counts.
/// </summary>
public sealed class EstablishmentFacetProvider : IFacetProvider
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _contextFactory;
    private readonly Dictionary<string, Expression<Func<Establishment, object>>> _facetSelectors;

    /// <summary>
    /// Creates a new facet provider using the supplied context factory and facet selector map.
    /// </summary>
    /// <param name="contextFactory">Factory used to create EF Core database contexts.</param>
    /// <param name="facetSelectors">
    /// A mapping of facet names to expressions selecting the facet value from an <see cref="Establishment"/>.
    /// </param>
    public EstablishmentFacetProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> contextFactory,
        Dictionary<string, Expression<Func<Establishment, object>>> facetSelectors)
    {
        _contextFactory = contextFactory;
        _facetSelectors = facetSelectors;
    }

    /// <summary>
    /// Computes facet buckets for the specified facet name across the supplied list of URNs.
    /// </summary>
    /// <param name="ids">The URNs to include in the facet calculation.</param>
    /// <param name="facetName">The facet name whose selector should be applied.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A read‑only list of <see cref="FacetResult"/> instances ordered by descending count.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the facet name is unknown or the database context cannot be created.
    /// </exception>
    public async Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IReadOnlyList<string> ids,
        string facetName,
        CancellationToken cancellationToken = default)
    {
        EducationProviderRegistryDbContext? context =
            await _contextFactory.CreateDbContextAsync(cancellationToken)
            ?? throw new InvalidOperationException("Failed to create database context.");

        await using (context)
        {
            if (!_facetSelectors.TryGetValue(facetName, out Expression<Func<Establishment, object>>? selector))
            {
                throw new InvalidOperationException($"Unknown facet '{facetName}'.");
            }

            IQueryable<Establishment> filtered =
                context.Establishment.Where(establishment =>
                    ids.Contains(establishment.Urn));

            IQueryable<IGrouping<object, Establishment>> grouped =
                filtered.GroupBy(selector);

            IQueryable<dynamic> sqlProjection =
                grouped.Select(groupedFacet => new
                {
                    groupedFacet.Key,
                    Count = groupedFacet.LongCount()
                });

            List<dynamic> rawFacetResults =
                await sqlProjection.ToListAsync(cancellationToken);

            List<FacetResult> results =
                [.. rawFacetResults
                    .Select(facetResult => new FacetResult(
                        facetResult.Key?.ToString() ?? string.Empty,
                        facetResult.Count))
                    .OrderByDescending(facet => facet.Count)];

            return results;
        }
    }
}
