using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;

/// <summary>
/// Defines how incoming filter keys map to filter expressions and the logical
/// operator used to combine them.
/// </summary>
public sealed class FilterKeyToFilterExpressionMapOptions
{
    /// <summary>
    /// Maps request filter keys to their corresponding filter expression options.
    /// </summary>
    [Required]
    [MinLength(1)]
    public IDictionary<string, FilterExpressionOptions> SearchFilterToExpressionMap { get; set; }
        = new Dictionary<string, FilterExpressionOptions>();
}

/// <summary>
/// Options describing how a filter expression should be constructed for a
/// specific request key.
/// </summary>
public sealed class FilterExpressionOptions
{
    /// <summary>
    /// The DI‑resolved filter expression type key.
    /// </summary>
    public string FilterExpressionKey { get; set; } = string.Empty;

    /// <summary>
    /// Optional delimiter used when formatting multi‑value filter expressions.
    /// </summary>
    public string FilterExpressionValuesDelimiter { get; set; } = string.Empty;

    /// <summary>
    /// The target property or field name that the filter expression should be applied to.
    /// </summary>
    public string FilterExpressionTarget { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether a delimiter has been specified.
    /// </summary>
    public bool HasValuesDelimiter =>
        !string.IsNullOrWhiteSpace(FilterExpressionValuesDelimiter);
}
