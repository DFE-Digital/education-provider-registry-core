namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Encapsulates a single facet result and count for a given fact type.
/// </summary>
public sealed class FacetResult
{
    /// <summary>
    /// The identifier/value used when applying this facet.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The human-readable value displayed for this facet.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// The number of records that belong to this facet value.
    /// </summary>
    public long? Count { get; }

    /// <summary>
    ///  Establishes an immutable <see cref="FacetResult"/> instance via the constructor arguments specified.
    /// </summary>
    /// <param name="label">
    /// The values associated with the given facet type.
    /// </param>
    /// <param name="count">
    /// The number of records that belong to this facet value.
    /// </param>
    public FacetResult(string value, string label, long? count)
    {
        Value = value;
        Label = label;
        Count = count;
    }
}
