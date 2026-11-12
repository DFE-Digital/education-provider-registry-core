using System.Globalization;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Extensions;

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

    public static DateRange ParseDateRange(this SearchFilterRequest request)
    {
        ValidateRange(request);

        List<ValidationErrorCode> errors = [];

        DateOnly? lower =
            ToDateOnly(
                request.FilterValues[0],
                ValidationErrorCode.LowerNotValid,
                errors);

        DateOnly? upper =
            request.FilterValues.Length == 2
                ? ToDateOnly(
                    request.FilterValues[1],
                    ValidationErrorCode.UpperNotValid,
                    errors)
                : null;

        if (errors.Count > 0)
        {
            throw new InvalidRangeException(errors);
        }

        if (lower.HasValue &&
                upper.HasValue &&
                    lower.Value > upper.Value)
        {
            throw new InvalidRangeException([ValidationErrorCode.UpperNotGreaterThanLower]);
        }

        BoundedRange<DateOnly> bounds = new(
            lower,
            upper);

        return new DateRange(bounds);
    }

    public static AgeRange ParseNumericRange(
        this SearchFilterRequest request)
    {
        ValidateRange(request);

        List<ValidationErrorCode> errors = [];

        int? lower =
            ToInteger(
                request.FilterValues[0],
                ValidationErrorCode.LowerNotValid,
                errors);

        int? upper =
            request.FilterValues.Length == 2 ?
                ToInteger(
                    request.FilterValues[1],
                    ValidationErrorCode.UpperNotValid,
                    errors) :
                null;

        if (errors.Count > 0)
        {
            throw new InvalidRangeException(errors);
        }

        if (lower.HasValue &&
            upper.HasValue &&
            lower.Value >= upper.Value)
        {
            throw new InvalidRangeException(
            [
                ValidationErrorCode.UpperNotGreaterThanLower
            ]);
        }

        BoundedRange<int> bounds = new(
            lower,
            upper);

        return new AgeRange(bounds);
    }

    private static DateOnly? ToDateOnly(
        object? value,
        ValidationErrorCode invalidValueError,
        List<ValidationErrorCode> errors)
    {
        switch (value)
        {
            case null:
                return null;

            case DateOnly dateOnly:
                return dateOnly;

            default:
                errors.Add(invalidValueError);
                return null;
        }
    }

    private static int? ToInteger(
        object? value,
        ValidationErrorCode invalidValueError,
        List<ValidationErrorCode> errors)
    {
        switch (value)
        {
            case null:
                return null;

            case int integer:
                return integer;

            default:
                errors.Add(invalidValueError);
                return null;
        }
    }

    private static void ValidateRange(SearchFilterRequest request)
    {
        if (request.FilterValues.Length is < 1 or > 2)
        {
            throw new ArgumentException(
                "Range filters require one or two filter values.");
        }
    }
}

