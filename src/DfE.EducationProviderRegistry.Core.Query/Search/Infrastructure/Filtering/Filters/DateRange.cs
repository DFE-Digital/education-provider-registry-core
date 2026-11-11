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
            errors.Add(ValidationErrorCode.LowerBoundBeforeMinimum);
        }

        if (boundedRange.Upper.HasValue &&
                boundedRange.Upper.Value < _minimumDate)
        {
            errors.Add(ValidationErrorCode.UpperBoundBeforeMinimum);
        }

        if (boundedRange.Lower.HasValue &&
                boundedRange.Upper.HasValue &&
                    boundedRange.Upper.Value <= boundedRange.Lower.Value)
        {
            errors.Add(ValidationErrorCode.UpperBoundNotAfterLowerBound);
        }

        if (errors.Count > 0)
        {
            throw new InvalidRangeException(errors);
        }

        Range = boundedRange;
    }

    public BoundedRange<DateOnly> Range { get; }
}
