using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Provides facet aggregation for <see cref="SearchAggregate"/> entities by grouping
/// an unpaginated matching query on a specified facet selector and returning bucket counts.
/// </summary>
public sealed class FacetProvider : IFacetProvider
{
    private readonly Dictionary<object, FacetDefinition<SearchAggregate>> _facetDefinitions;

    /// <summary>
    /// Initializes a new instance of the FacetProvider class with the specified facet definitions.
    /// </summary>
    /// <param name="facetDefinitions">A dictionary that maps objects to FacetDefinition<SearchAggregate> instances, representing the facet
    /// definitions.</param>
    public FacetProvider(Dictionary<object, FacetDefinition<SearchAggregate>> facetDefinitions)
    {
        _facetDefinitions = facetDefinitions;
    }

    /// <summary>
    /// Computes facet buckets for the specified facet name across the supplied list of URNs.
    /// </summary>
    /// <param name="matchingQuery">The URNs to include in the facet calculation.</param>
    /// <param name="facetName">The facet name whose selector should be applied.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A read‑only list of <see cref="FacetResult"/> instances ordered by descending count.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the facet name is unknown or the database context cannot be created.
    /// </exception>
    public async Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        string facetName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(matchingQuery);

        if (!_facetDefinitions.TryGetValue(facetName, out FacetDefinition<SearchAggregate>? facetDefinition))
        {
            throw new InvalidOperationException($"Unknown facet '{facetName}'.");
        }

        IQueryable<IGrouping<object, SearchAggregate>> grouped =
            matchingQuery.GroupBy(facetDefinition.ValueSelector);

        IQueryable<dynamic> sqlProjection =
            grouped.Select(groupedFacet => new
            {
                Value = groupedFacet.Key,
                Count = groupedFacet.LongCount(),
                Label = groupedFacet
                    .AsQueryable()
                    .Select(facetDefinition.LabelSelector).FirstOrDefault(),
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
