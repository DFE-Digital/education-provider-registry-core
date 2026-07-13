using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal class FacetProviderTestDouble
{
    public static Mock<IFacetProvider> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<IFacetProvider>
        MockFor(IReadOnlyList<FacetResult> facetResults)
    {
        Mock<IFacetProvider> facetProviderMock = Mock();

        facetProviderMock
            .Setup(facetProvider => facetProvider.GetFacetsAsync(
                It.IsAny<ReadOnlyCollection<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(facetResults);

        return facetProviderMock;
    }
}
