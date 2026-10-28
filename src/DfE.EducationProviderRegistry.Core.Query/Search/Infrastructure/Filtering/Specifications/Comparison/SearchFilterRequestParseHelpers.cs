using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Specifications.Comparison;

public static class SearchFilterRequestParseHelpers
{
    internal static (DateOnly from, DateOnly to) ParseFilterRequestDates(SearchFilterRequest request)
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

        if (values.Count == 0)
        {
            throw new ArgumentException("No parsed values to filter in FilterRequestValues");
        }

        if (values.Count > 2)
        {
            throw new ArgumentException("More values than comparable for DateFilterRange");
        }

        return (values[0], values[1]);
    }
}
