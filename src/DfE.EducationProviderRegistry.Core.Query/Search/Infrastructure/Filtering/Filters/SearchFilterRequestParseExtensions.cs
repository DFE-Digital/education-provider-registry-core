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
            throw new ArgumentException("No values to filter in FilterRequestValues.");
        }

        if (request.FilterValues.Length > 2)
        {
            throw new ArgumentException("More values than comparable for DateFilterRange.");
        }

        DateOnly? lower = ToDateOnly(request.FilterValues[0]);

        DateOnly? upper =
            request.FilterValues.Length == 2 ?
                ToDateOnly(request.FilterValues[1])
                    : null;

        BoundedRange<DateOnly> bounds = new(lower, upper);

        return new(bounds);
    }

    private static DateOnly? ToDateOnly(object? value)
    {
        return value switch
        {
            null => null,
            DateOnly dateOnly => dateOnly,

            _ => throw new ArgumentException(
                $"Unsupported filter value type '{value.GetType().Name}'.")
        };
    }
}

