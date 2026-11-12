using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed record AgeRange
{
    private const int MinimumAge = 0;
    private const int MaximumAge = 99;

    public AgeRange(BoundedRange<int> boundedRange)
    {
        List<ValidationErrorCode> errors = [];

        if (boundedRange.Lower.HasValue &&
                boundedRange.Lower.Value < MinimumAge)
        {
            errors.Add(
                ValidationErrorCode.LowerBeforeMinimum);
        }

        if (boundedRange.Upper.HasValue &&
                boundedRange.Upper.Value < MinimumAge)
        {
            errors.Add(
                ValidationErrorCode.UpperBeforeMinimum);
        }

        if (boundedRange.Lower.HasValue &&
                boundedRange.Lower.Value > MaximumAge)
        {
            errors.Add(
                ValidationErrorCode.LowerAfterMaximum);
        }

        if (boundedRange.Upper.HasValue &&
                boundedRange.Upper.Value > MaximumAge)
        {
            errors.Add(ValidationErrorCode.UpperAfterMaximum);
        }

        if (errors.Count > 0)
        {
            throw new InvalidRangeException(errors);
        }

        Range = boundedRange;
    }

    public BoundedRange<int> Range { get; }
}
