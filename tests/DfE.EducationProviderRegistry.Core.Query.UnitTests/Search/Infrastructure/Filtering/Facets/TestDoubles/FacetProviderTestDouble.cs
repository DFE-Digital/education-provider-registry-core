using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets.TestDoubles;

public static class FacetProviderTestDouble
{
    public static Mock<IFacetProvider> Mock() => new(MockBehavior.Strict);

    public static Mock<IFacetProvider> MockFor(
        IReadOnlyDictionary<string, IReadOnlyList<FacetResult>>? facetMap = null,
        string? throwKey = null,
        Exception? exception = null)
    {
        Mock<IFacetProvider> mock = Mock();

        mock.Setup(facetProvider =>
            facetProvider.GetFacetsAsync(
                It.IsAny<IReadOnlyList<string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<string> urns, string facetKey, CancellationToken _) =>
            {
                if (throwKey != null &&
                    facetKey.Equals(throwKey, StringComparison.OrdinalIgnoreCase))
                {
                    throw exception ??
                        new InvalidOperationException("Test exception");
                }

                if (facetMap != null)
                {
                    foreach (KeyValuePair<string, IReadOnlyList<FacetResult>> kvp in facetMap)
                    {
                        if (facetKey.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            return kvp.Value;
                        }
                    }
                }

                return Array.Empty<FacetResult>();
            })
            .Verifiable();

        return mock;
    }
}
