namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed record DateFilterRange
{
    private static readonly DateOnly _minValue = new(1800, 1, 1);

    public DateFilterRange(BoundedRange<DateOnly> boundedRange)
    {
        if (boundedRange.Lower.HasValue)
        {
            Validate(boundedRange.Lower.Value);
        }

        if (boundedRange.Upper.HasValue)
        {
            Validate(boundedRange.Upper.Value);
        }

        Range = boundedRange;
    }

    public BoundedRange<DateOnly> Range { get; }

    private static void Validate(DateOnly date)
    {
        if (date < _minValue)
        {
            throw new ArgumentException(
                "Filter date must be greater than or equal to 1800-01-01.");
        }
    }
}
