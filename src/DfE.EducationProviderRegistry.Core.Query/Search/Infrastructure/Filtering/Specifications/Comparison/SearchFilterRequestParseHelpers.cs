using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Specifications.Comparison;

public static class SearchFilterRequestParseHelpers
{
    internal static IReadOnlyCollection<DateOnly> ParseFilterRequestDates(SearchFilterRequest request)
    {
        List<DateOnly> values = [.. request.FilterValues
            .Where(filterValue => filterValue is not null)
            .Select(filterValue => filterValue switch
            {
                DateTime dateTime => DateOnly.FromDateTime(dateTime),
                string value => DateOnly.Parse(
                    value,
                    CultureInfo.GetCultureInfo("en-GB")),

                _ => throw new ArgumentException(
                    $"Unsupported filter value type '{filterValue.GetType().Name}'.")
            })];

        return values;
    }
}
