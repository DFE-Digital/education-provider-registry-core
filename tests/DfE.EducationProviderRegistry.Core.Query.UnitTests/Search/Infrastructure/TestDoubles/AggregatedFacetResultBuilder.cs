using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

internal sealed class AggregatedFacetResultBuilder
{
    private string _name = "TestFacet";
    private readonly List<FacetResult> _results = [];

    public static AggregatedFacetResultBuilder Create() => new();

    public AggregatedFacetResultBuilder WithName(string value)
    {
        _name = value;
        return this;
    }

    public AggregatedFacetResultBuilder WithFacetResult(string value, string label, int count)
    {
        _results.Add(
            new(value, label, count));

        return this;
    }

    public AggregatedFacetResult Build()
    {
        return new(
            _name,
            _results);
    }
}
