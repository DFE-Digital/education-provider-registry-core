using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Options.TestDoubles;

[ExcludeFromCodeCoverage]
public static class FilterKeyToFilterExpressionMapOptionsStub
{

    public static FilterKeyToFilterExpressionMapOptions ValidSingle()
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>
            {
                { "Key1", new FilterExpressionOptions() }
            }
        };
    }

    public static FilterKeyToFilterExpressionMapOptions EmptyMap()
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            SearchFilterToExpressionMap = new Dictionary<string, FilterExpressionOptions>()
        };
    }

    public static IOptions<FilterKeyToFilterExpressionMapOptions> EmptyMapIOptions() => Microsoft.Extensions.Options.Options.Create(EmptyMap());

}
