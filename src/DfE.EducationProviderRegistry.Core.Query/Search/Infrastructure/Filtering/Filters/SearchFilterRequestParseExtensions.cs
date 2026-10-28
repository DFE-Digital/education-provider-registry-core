using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal static class SearchFilterRequestParseExtensions
{
    /// <summary>
    /// Provides extension methods for parsing values from a search filter request.
    /// </summary>
    /// <remarks>
    /// Range filter values are interpreted positionally:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             <c>[lower]</c> specifies a lower bound only.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <c>[lower, null]</c> specifies a lower bound only.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <c>[null, upper]</c> specifies an upper bound only.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <c>[lower, upper]</c> specifies both lower and upper bounds.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <c>[null, null]</c> does not define a valid range.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    public static DateRange ParseFilterRequestDates(this SearchFilterRequest request)
    {
        if (request.FilterValues.Length == 0)
        {
            throw new ArgumentException("No parsed values to filter in FilterRequestValues");
        }

        if (request.FilterValues.Length > 2)
        {
            throw new ArgumentException("More values than comparable for DateFilterRange");
        }

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

