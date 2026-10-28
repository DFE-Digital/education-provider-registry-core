using System.Globalization;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Specifications.Equals;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class SearchProviderTypeFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        IReadOnlyCollection<string?> values =
        [
            .. request.FilterValues
                .Where((filterValue) => filterValue is not null)
                .Select(ConvertFilterValue)
        ];

        return new PropertyEqualsAnyValuesSpecification<SearchAggregate, string?>(
            searchProvider => searchProvider.ProviderTypeName,
            values);
    }

    private static string ConvertFilterValue(object value)
    {
        return value switch
        {
            string str => str,
            byte or sbyte or
            short or ushort or
            int or uint or
            long or ulong
                or float or double or decimal
                => ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture),
            _ => throw new ArgumentException(
                $"Unsupported filter value type '{value.GetType()}'.")
        };
    }
}
