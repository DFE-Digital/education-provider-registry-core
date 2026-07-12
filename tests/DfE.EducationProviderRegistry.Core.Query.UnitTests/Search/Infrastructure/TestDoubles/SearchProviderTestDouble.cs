using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchProviderTestDouble
{
    public static Mock<ISearchProvider<Establishment>> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<ISearchProvider<Establishment>>
        MockFor(List<Establishment> establishments)
    {
        Mock<ISearchProvider<Establishment>> searchProviderMock = Mock();

        searchProviderMock
            .Setup(searchProvider =>
                searchProvider.GetMatchingIdsAsync(
                It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IReadOnlyList<SearchFilterRequest>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(establishments);

        return searchProviderMock;
    }
}
