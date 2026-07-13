using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;

/// <summary>
/// Configuration options for mapping incoming filter request keys to the
/// corresponding filter expression and logical operator used to compose
/// a complete filter expression. This allows callers to specify which
/// filter expression type should be used for a given request key, along
/// with the logical operator used to chain multiple filter expressions
/// together.
/// 
/// Example configuration:
/// <code>
/// "FilterKeyToFilterExpressionMapOptions": {
///     "FilterChainingLogicalOperator": "AndLogicalOperator",
///     "SearchFilterToExpressionMap": {
///         "RELIGIOUSCHARACTERCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter": ","
///         },
///         "OFSTEDRATINGCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter": ","
///         }
///     }
/// }
/// </code>
/// 
/// When filter values are supplied for the keys above, the configured
/// filter expression types and logical operator are used to construct
/// a combined filter expression appropriate for the underlying search
/// provider (SQL, trigram, Azure Search, or others).
/// </summary>
public sealed class FilterKeyToFilterExpressionMapOptions
{
    /// <summary>
    /// The logical operator used to chain multiple filter expressions together.
    /// Typically configured as either <b>AndLogicalOperator</b> or
    /// <b>OrLogicalOperator</b>.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string? FilterChainingLogicalOperator { get; set; }

    /// <summary>
    /// A dictionary mapping incoming filter request keys to the filter
    /// expression configuration that should be applied.
    /// </summary>
    [Required]
    [MinLength(1)]
    public IDictionary<string, FilterExpressionOptions> SearchFilterToExpressionMap { get; set; }
        = new Dictionary<string, FilterExpressionOptions>();
}

/// <summary>
/// Configuration options describing how a specific filter expression should
/// be constructed for a given request key. This includes the filter expression
/// type to resolve (e.g., <c>SearchInFilterExpression</c>,
/// <c>LessThanOrEqualToExpression</c>) and an optional delimiter used when
/// formatting multiple values within the expression.
/// </summary>
public sealed class FilterExpressionOptions
{
    /// <summary>
    /// The key used to resolve the filter expression type from the DI container.
    /// By convention, this corresponds to the class name of the filter expression
    /// implementation.
    /// </summary>
    public string FilterExpressionKey { get; set; } = string.Empty;

    /// <summary>
    /// Optional delimiter applied between multiple values when constructing
    /// filter expressions that require value separation (e.g., comma‑separated
    /// lists).
    /// </summary>
    public string FilterExpressionValuesDelimiter { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether a delimiter has been specified.
    /// </summary>
    public bool HasValuesDelimiter => !string.IsNullOrWhiteSpace(FilterExpressionValuesDelimiter);
}
