using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed record DateRange
{
    private static readonly DateOnly _minimumDate = new(1800, 1, 1);

    public DateRange(BoundedRange<DateOnly> boundedRange)
    {
        List<ValidationErrorCode> errors = [];

        if (boundedRange.Lower.HasValue &&
            boundedRange.Lower.Value < _minimumDate)
        {
            errors.Add(
                ValidationErrorCode.LowerBeforeMinimum);
        }

        if (boundedRange.Upper.HasValue &&
            boundedRange.Upper.Value < _minimumDate)
        {
            errors.Add(
                ValidationErrorCode.UpperBeforeMinimum);
        }

        if (errors.Count > 0)
        {
            throw new InvalidRangeException(errors);
        }

        Range = boundedRange;
    }

    public BoundedRange<DateOnly> Range { get; }
}
