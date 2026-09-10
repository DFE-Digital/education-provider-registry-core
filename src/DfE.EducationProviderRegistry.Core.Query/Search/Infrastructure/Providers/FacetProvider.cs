using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Provides facet aggregation for <see cref="SearchAggregate"/> entities by grouping
/// a filtered set of URNs on a specified facet selector and returning bucket counts.
/// </summary>
public sealed class FacetProvider : IFacetProvider
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _contextFactory;
    private readonly Dictionary<string, FacetDefinition<SearchAggregate>> _facetDefinitions;

    /// <summary>
    /// Creates a new facet provider using the supplied context factory and facet selector map.
    /// </summary>
    /// <param name="contextFactory">Factory used to create EF Core database contexts.</param>
    /// <param name="facetSelectors">
    /// A mapping of facet names to expressions selecting the facet value from an <see cref="Establishment"/>.
    /// </param>
    public FacetProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> contextFactory,
        Dictionary<string, FacetDefinition<SearchAggregate>> facetDefinitions)
    {
        _contextFactory = contextFactory;
        _facetDefinitions = facetDefinitions;
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
            if (!_facetDefinitions.TryGetValue(facetName, out FacetDefinition<SearchAggregate>? facetDefinition))
            {
                throw new InvalidOperationException($"Unknown facet '{facetName}'.");
            }

            IQueryable<SearchAggregate> filtered =
                context.SearchAggregate
                .Where(sp => ids.Contains(sp.ProviderId));

            IQueryable<IGrouping<object, SearchAggregate>> grouped =
                filtered.GroupBy(facetDefinition.ValueSelector);

            IQueryable<dynamic> sqlProjection =
                grouped.Select(groupedFacet => new
                {
                    Value =
                        groupedFacet.Key,
                    Label =
                        groupedFacet.AsQueryable()
                            .Select(facetDefinition.LabelSelector).FirstOrDefault(),
                    Count =
                        groupedFacet.LongCount()
                });

            List<dynamic> rawFacetResults =
                await sqlProjection.ToListAsync(cancellationToken);

            List<FacetResult> results =
                [.. rawFacetResults
                    .Select(facetResult => new FacetResult(
                        facetResult.Value?.ToString() ?? string.Empty,
                        facetResult.Label ?? string.Empty,
                        facetResult.Count
                    ))
                    .OrderByDescending(facet => facet.Count)];

            return results;
        }
    }
}
