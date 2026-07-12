namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

public sealed class SearchFilterRequest
{
    public string FilterKey { get; }

    public object[] FilterValues { get; }

    public string FilterValuesDelimiter { get; private set; } = string.Empty;

    public SearchFilterRequest(string filterKey, IEnumerable<object> filterValues)
    {
        ArgumentException.ThrowIfNullOrEmpty(filterKey);
        ArgumentNullException.ThrowIfNull(filterValues);

        if (!filterValues.Any())
        {
            throw new ArgumentException(
                "Filter values are required to build search filter arguments", nameof(filterValues));
        }

        FilterKey = filterKey;
        FilterValues = [.. filterValues];
    }

    public void SetFilterValuesDelimiter(string filterValuesDelimiter)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(filterValuesDelimiter);

        FilterValuesDelimiter = filterValuesDelimiter;
    }
}
