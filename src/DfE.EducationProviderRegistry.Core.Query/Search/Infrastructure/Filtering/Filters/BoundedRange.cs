namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public readonly record struct BoundedRange<T> where T : struct, IComparable<T>
{
    public BoundedRange(T? lower, T? upper)
    {
        if (!lower.HasValue && !upper.HasValue)
        {
            throw new ArgumentException("Bounded range must have a value");
        }

        if (lower.HasValue &&
                upper.HasValue &&
                    lower.Value.CompareTo(upper.Value) > 0)
        {
            throw new ArgumentException("Right must be greater than or equal to Left.");
        }

        Lower = lower;
        Upper = upper;
    }

    public T? Lower { get; }
    public T? Upper { get; }

    public bool HasLowerAndUpperBounds => HasLowerBounds && HasUpperBounds;
    public bool HasLowerBounds => Lower.HasValue;
    public bool HasUpperBounds => Upper.HasValue;
}
