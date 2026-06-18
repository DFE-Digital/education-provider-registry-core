using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;

/// <summary>
/// A dummy implementation of <see cref="ISearchServiceAdapter{TResults, TFacets}"/>
/// that returns deterministic mock data for development, UI prototyping,
/// and integration testing.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class DummySearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    /// <summary>
    /// Generates a fixed set of 200 mock establishments along with
    /// predefined facet groups. This method ignores the incoming request
    /// and always returns the same deterministic dataset.
    /// </summary>
    public Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest searchServiceAdapterRequest,
        CancellationToken cancellationToken = default)
    {
        List<EstablishmentSearchResult> establishments = new();

        for (int i = 1; i <= 200; i++)
        {
            establishments.Add(
                EstablishmentSearchResult.Create(
                    urn: new UniqueReferenceNumber((100000 + i).ToString()),
                    name: new Name($"Mock Establishment {i}"),
                    address: new Address(
                        Street: "123 Example Street",
                        Town: "Testville",
                        County: "Testshire",
                        Postcode: "TE5 7ST"),
                    type: EstablishmentType.Create("Academy"),
                    group: GroupDetail.Create("Mock Trust", "TRUST001"),
                    localAuthority: LocalAuthority.Create("Test LA", "LA001")
                ));
        }

        EstablishmentSearchResults establishmentResults = new(establishments);

        List<SearchFacet> facetList =
        [
            new SearchFacet(
                "Region",
                [
                    new FacetResult("North", 80),
                    new FacetResult("South", 60),
                    new FacetResult("Midlands", 60)
                ]),

            new SearchFacet(
                "ProviderType",
                [
                    new FacetResult("Academy", 120),
                    new FacetResult("LA Maintained", 50),
                    new FacetResult("Independent", 30)
                ])
        ];

        SearchFacets facets = new(facetList);

        SearchResults<EstablishmentSearchResults, SearchFacets> response =
            new()
            {
                Results = establishmentResults,
                FacetResults = facets
            };

        return Task.FromResult(response);
    }
}
