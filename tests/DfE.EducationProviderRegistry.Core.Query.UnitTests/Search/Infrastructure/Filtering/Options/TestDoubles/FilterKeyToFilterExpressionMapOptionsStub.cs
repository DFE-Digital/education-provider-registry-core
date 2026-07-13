using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Options.TestDoubles;

[ExcludeFromCodeCoverage]
public static class FilterKeyToFilterExpressionMapOptionsStub
{
    public static IOptions<FilterKeyToFilterExpressionMapOptions> CreateIOptions(
        FilterKeyToFilterExpressionMapOptions options)
    {
        return Microsoft.Extensions.Options.Options.Create(options);
    }

    public static FilterKeyToFilterExpressionMapOptions Create(
        string chainingOperator,
        Dictionary<string, FilterExpressionOptions> map)
    {
        FilterKeyToFilterExpressionMapOptions options = new()
        {
            FilterChainingLogicalOperator = chainingOperator,
            SearchFilterToExpressionMap = map
        };

        return options;
    }

    public static FilterKeyToFilterExpressionMapOptions ValidSingle()
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            FilterChainingLogicalOperator = "AND",
            SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>
            {
                { "Key1", new FilterExpressionOptions() }
            }
        };
    }

    public static FilterKeyToFilterExpressionMapOptions ValidWith(
        params (string key, FilterExpressionOptions expr)[] entries)
    {
        Dictionary<string, FilterExpressionOptions> map =
            entries.ToDictionary(options =>
                options.key,
                options => options.expr);

        return new FilterKeyToFilterExpressionMapOptions
        {
            FilterChainingLogicalOperator = "AND",
            SearchFilterToExpressionMap = map
        };
    }

    public static FilterKeyToFilterExpressionMapOptions MissingChainingOperator()
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            FilterChainingLogicalOperator = null,
            SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>
            {
                { "Key1", new FilterExpressionOptions() }
            }
        };
    }

    public static IOptions<FilterKeyToFilterExpressionMapOptions> MissingChainingOperatorIOptions() =>
        CreateIOptions(MissingChainingOperator());

    public static FilterKeyToFilterExpressionMapOptions EmptyMap()
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            FilterChainingLogicalOperator = "AND",
            SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>()
        };
    }

    public static IOptions<FilterKeyToFilterExpressionMapOptions> EmptyMapIOptions() => CreateIOptions(EmptyMap());

    public static FilterKeyToFilterExpressionMapOptions WithChaining(string chainingOperator)
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            FilterChainingLogicalOperator = chainingOperator,
            SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>
            {
                { "Key1", new FilterExpressionOptions() }
            }
        };
    }
}
