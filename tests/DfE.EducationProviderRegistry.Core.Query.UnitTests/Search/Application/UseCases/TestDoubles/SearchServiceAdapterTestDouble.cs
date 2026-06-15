using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
public class SearchServiceAdapterTestDouble
{
    private readonly Mock<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>> _mock = new();

    public Mock<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>> MockFor(
        SearchResults<EstablishmentSearchResults, SearchFacets> searchResults)
    {
        _mock
            .Setup(adapter => adapter.SearchAsync(
                It.IsAny<SearchServiceAdapterRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResults)
            .Verifiable();

        return _mock;
    }
}
