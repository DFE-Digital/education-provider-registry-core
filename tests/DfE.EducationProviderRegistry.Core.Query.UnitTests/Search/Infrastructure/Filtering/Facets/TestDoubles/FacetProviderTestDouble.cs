using System.Collections.Concurrent;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets.TestDoubles;

public static class FacetProviderTestDouble
{
    /// <summary>
    /// Creates a strict mock of IFacetProvider with optional preconfigured facet responses.
    /// </summary>
    public static Mock<IFacetProvider> Mock(
        Action<FacetProviderBuilder>? configure = null)
    {
        Mock<IFacetProvider> mock = new Mock<IFacetProvider>(MockBehavior.Strict);
        FacetProviderBuilder builder = new FacetProviderBuilder(mock);

        configure?.Invoke(builder);

        builder.Apply();

        return mock;
    }

    /// <summary>
    /// Fluent builder for configuring facet responses.
    /// </summary>
    public sealed class FacetProviderBuilder
    {
        private readonly Mock<IFacetProvider> _mock;

        // Key: facet name (case-insensitive)
        private readonly ConcurrentDictionary<string, IReadOnlyList<FacetResult>> _responses =
            new(StringComparer.OrdinalIgnoreCase);

        public FacetProviderBuilder(Mock<IFacetProvider> mock)
        {
            _mock = mock;
        }

        public FacetProviderBuilder Returns(string facetName, IReadOnlyList<FacetResult> results)
        {
            _responses[facetName] = results;
            return this;
        }

        public FacetProviderBuilder Throws(string facetName, Exception ex)
        {
            _responses[facetName] = null!;
            _mock
                .Setup(p => p.GetFacetsAsync(It.IsAny<IReadOnlyList<string>>(), facetName, It.IsAny<CancellationToken>()))
                .ThrowsAsync(ex);

            return this;
        }

        internal void Apply()
        {
            foreach (KeyValuePair<string, IReadOnlyList<FacetResult>> kvp in _responses)
            {
                if (kvp.Value is null)
                    continue;

                string facetName = kvp.Key;
                IReadOnlyList<FacetResult> results = kvp.Value;

                _mock
                    .Setup(p => p.GetFacetsAsync(
                        It.IsAny<IReadOnlyList<string>>(),
                        facetName,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(results);
            }
        }
    }
}
