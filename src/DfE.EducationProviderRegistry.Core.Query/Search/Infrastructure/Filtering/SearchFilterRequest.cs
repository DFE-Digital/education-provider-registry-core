namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

/// <summary>
/// Represents a filter request consisting of a filter key and one or more
/// values used to construct a search filter expression.
/// </summary>
public sealed class SearchFilterRequest
{
    /// <summary>
    /// The key identifying which filter expression should be applied.
    /// </summary>
    public string FilterKey { get; }

    /// <summary>
    /// The values supplied for the filter expression.
    /// </summary>
    public object[] FilterValues { get; }

    /// <summary>
    /// Optional delimiter used when formatting multi‑value filter expressions.
    /// </summary>
    public string FilterValuesDelimiter { get; private set; } = string.Empty;

    /// <summary>
    /// Creates a new filter request with the specified key and values.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when the key is empty or no values are provided.
    /// </exception>
    public SearchFilterRequest(string filterKey, IEnumerable<object> filterValues)
    {
        ArgumentException.ThrowIfNullOrEmpty(filterKey);
        ArgumentNullException.ThrowIfNull(filterValues);

        if (!filterValues.Any())
        {
            throw new ArgumentException(
                "Filter values are required to build search filter arguments",
                nameof(filterValues));
        }

        FilterKey = filterKey;
        FilterValues = [.. filterValues];
    }

    /// <summary>
    /// Sets the delimiter used when formatting multiple filter values.
    /// </summary>
    public void SetFilterValuesDelimiter(string filterValuesDelimiter)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(filterValuesDelimiter);
        FilterValuesDelimiter = filterValuesDelimiter;
    }
}
