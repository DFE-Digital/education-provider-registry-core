namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;

public sealed record DateRange
{
    private static DateOnly MinimumExclusive { get; } = new(1800, 1, 1);

    public DateOnly? From { get; }
    public DateOnly? To { get; }
    public bool HasBounds => From.HasValue || To.HasValue;

    public DateRange(DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && from.Value <= MinimumExclusive)
            throw new ArgumentOutOfRangeException(nameof(from), "Date must be after 01/01/1800");

        if (to.HasValue && to.Value <= MinimumExclusive)
            throw new ArgumentOutOfRangeException(nameof(to), "Date must be after 01/01/1800");

        if (from.HasValue && to.HasValue && from.Value > to.Value)
            throw new ArgumentException("The from date must be on or before the to date.", nameof(from));

        From = from;
        To = to;
    }
}
