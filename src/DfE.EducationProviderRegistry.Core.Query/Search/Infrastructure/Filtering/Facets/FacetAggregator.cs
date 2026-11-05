using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;


public sealed class FacetAggregator : IFacetAggregator
{
    private readonly IReadOnlyDictionary<string, IFacetResultProvider> _facetResultProviders;

    public FacetAggregator(IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders)
    {
        ArgumentNullException.ThrowIfNull(facetResultProviders);
        _facetResultProviders = facetResultProviders;
    }

    public async Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        IReadOnlyCollection<FacetFilterContext> filters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(matchingQuery);

        if (filters is null || filters.Count == 0)
        {
            return [];
        }

        List<FacetProjection> projections =
            await matchingQuery
                .Select(searchAggregate => new FacetProjection
                {
                    Admissions = searchAggregate.Admissions,
                    Boarding = searchAggregate.Boarding,
                    Diocese = searchAggregate.Diocese,
                    EducationPhase = searchAggregate.EducationPhase,
                    FurtherEducationType = searchAggregate.FurtherEducationType,
                    Gender = searchAggregate.Gender,
                    GroupType = searchAggregate.GroupType,
                    Nursery = searchAggregate.Nursery,
                    ReligiousCharacter = searchAggregate.ReligiousCharacter,
                    ReligiousEthos = searchAggregate.ReligiousEthos,
                    ProviderTypeName = searchAggregate.ProviderTypeName,
                    Section41 = searchAggregate.Section41,
                    Sen = searchAggregate.Sen,
                    SixthForm = searchAggregate.SixthForm,
                    SpecialClass = searchAggregate.SpecialClass,
                    StatusName = searchAggregate.StatusName,
                })
                .ToListAsync(cancellationToken);

        List<AggregatedFacetResult> results = [];

        foreach (FacetFilterContext facetContext in filters)
        {
            if (!_facetResultProviders.TryGetValue(facetContext.Facet, out IFacetResultProvider? provider))
            {
                throw new InvalidOperationException($"Unknown facet '{facetContext.Facet}'.");
            }

            IReadOnlyList<FacetResult> facetResults =
                provider.GetFacets(
                    projections,
                    [.. facetContext.Filter?.FilterValues ?? []]);

            results.Add(
                new AggregatedFacetResult(
                    facetContext.Facet,
                    facetResults));
        }

        return results;
    }
}


public sealed class FacetProjection
{
    public string? Admissions { get; init; }

    public string? Boarding { get; init; }

    public string? Diocese { get; init; }

    public string? EducationPhase { get; init; }

    public string? FurtherEducationType { get; init; }

    public string? Gender { get; init; }

    public string? GroupType { get; init; }

    public string? Nursery { get; init; }

    public string? ReligiousCharacter { get; init; }

    public string? ReligiousEthos { get; init; }

    public string? ProviderTypeName { get; init; }

    public List<string>? Sen { get; init; }

    public string? Section41 { get; init; }

    public string? SixthForm { get; init; }

    public string? StatusName { get; init; }

    public string? SpecialClass { get; init; }
}
