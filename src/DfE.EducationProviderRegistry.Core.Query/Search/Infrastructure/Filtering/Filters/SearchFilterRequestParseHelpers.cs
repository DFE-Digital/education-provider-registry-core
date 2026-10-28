using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal static class SearchFilterRequestParseHelpers
{
    public static DateRange ParseFilterRequestDates(SearchFilterRequest request)
    {
        if (request.FilterValues.Length == 0)
        {
            throw new ArgumentException("No parsed values to filter in FilterRequestValues");
        }

        if (request.FilterValues.Length > 2)
        {
            throw new ArgumentException("More values than comparable for DateFilterRange");
        }

        /*
         *  new object?[] { date1 }         // Lower only
            new object?[] { date1, null }   // Lower only
            new object?[] { null, date2 }   // Upper only
            new object?[] { date1, date2 }  // Both
            new object?[] { null, null }    // Invalid via BoundedRange
         */

        DateOnly? lower = ParseDate(request.FilterValues[0]);

        DateOnly? upper = request.FilterValues.Length == 2 ? ParseDate(request.FilterValues[1]) : null;

        BoundedRange<DateOnly> bounds = new(lower, upper);

        return new(bounds);
    }

    private static DateOnly? ParseDate(object? input)
    {
        return input switch
        {
            null => null,
            DateTime dateTime => DateOnly.FromDateTime(dateTime),
            string value => DateOnly.ParseExact(
                value,
                ["yyyy-MM-dd"],
                CultureInfo.InvariantCulture),
            _ => throw new ArgumentException(
                $"Unsupported filter value type '{input.GetType().Name}'.")
        };
    }
}

