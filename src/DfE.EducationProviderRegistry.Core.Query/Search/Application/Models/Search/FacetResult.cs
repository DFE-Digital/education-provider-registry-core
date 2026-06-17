namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Encapsulates a single facet result and count for a given fact type.
/// </summary>
public sealed class FacetResult
{
    /// <summary>
    /// The value of the facet result.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The number of records that belong to this facet value.
    /// </summary>
    public long? Count { get; }


    /// <summary>
    ///  Establishes an immutable <see cref="FacetResult"/> instance via the constructor arguments specified.
    /// </summary>
    /// <param name="value">
    /// The values associated with the given facet type.
    /// </param>
    /// <param name="count">
    /// The number of records that belong to this facet value.
    /// </param>
    public FacetResult(string value, long? count)
    {
        Value = value;
        Count = count;
    }
}
