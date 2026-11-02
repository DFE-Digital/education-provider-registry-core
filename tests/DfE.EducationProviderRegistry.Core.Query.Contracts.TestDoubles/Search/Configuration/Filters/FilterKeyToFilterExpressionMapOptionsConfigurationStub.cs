using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Filters;

internal static class FilterKeyToFilterExpressionMapOptionsStub
{
    public static FilterKeyToFilterExpressionMapOptions Stub()
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            SearchFilterToExpressionMap =
                new Dictionary<string, FilterExpressionOptions>
                {
                    {
                        "STUB",
                        new FilterExpressionOptions
                        {
                            FilterExpressionKey = "STUBBED_FILTER"
                        }
                    }
                }
        };
    }

    public static FilterKeyToFilterExpressionMapOptions Stub(IEnumerable<KeyValuePair<string, string?>> filters)
    {
        ArgumentNullException.ThrowIfNull(filters);

        return new FilterKeyToFilterExpressionMapOptions
        {
            SearchFilterToExpressionMap =
                filters.ToDictionary(
                    filter => filter.Key,
                    filter => new FilterExpressionOptions
                    {
                        FilterExpressionKey = filter.Value!
                    })
        };
    }
}
