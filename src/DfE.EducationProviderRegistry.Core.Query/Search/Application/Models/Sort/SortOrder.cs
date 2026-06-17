namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

/// <summary>
/// Represents a validated combination of a sort field and sort direction,
/// producing a fully normalized sort expression suitable for use by the
/// underlying search provider.
/// </summary>
public sealed class SortOrder
{
    /// <summary>
    /// The validated field to sort by.
    /// </summary>
    private readonly SortField _field;

    /// <summary>
    /// The validated direction of sorting (<c>"asc"</c> or <c>"desc"</c>).
    /// </summary>
    private readonly SortDirection _direction;

    /// <summary>
    /// Gets the combined sort expression in the format
    /// <c>"{field} {direction}"</c>, normalized for use in search queries.
    /// </summary>
    public string Value => $"{_field.Field} {_direction.Direction}";

    /// <summary>
    /// Initializes a new instance of the <see cref="SortOrder"/> class
    /// by validating both the sort field and sort direction.
    /// </summary>
    /// <param name="sortField">The field name to sort by.</param>
    /// <param name="sortDirection">The direction to sort in (<c>"asc"</c> or <c>"desc"</c>).</param>
    /// <param name="validSortFields">The list of allowed field names for sorting.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="sortField"/> is not in the allowed list,
    /// or when <paramref name="sortDirection"/> is not a valid direction.
    /// </exception>
    public SortOrder(string sortField, string sortDirection, IReadOnlyList<string> validSortFields)
    {
        _field = SortField.Create(sortField, validSortFields);
        _direction = SortDirection.Create(sortDirection);
    }

    /// <summary>
    /// Creates a new validated <see cref="SortOrder"/> instance.
    /// </summary>
    /// <param name="field">The field name to sort by.</param>
    /// <param name="direction">The direction to sort in.</param>
    /// <param name="validFields">The list of allowed field names.</param>
    /// <returns>
    /// A validated <see cref="SortOrder"/> instance.
    /// </returns>
    public static SortOrder Create(
        string field,
        string direction,
        IReadOnlyList<string> validFields) => new(field, direction, validFields);

    /// <summary>
    /// Returns the normalized sort expression.
    /// </summary>
    /// <returns>
    /// A string representing the sort order in the format
    /// <c>"{field} {direction}"</c>.
    /// </returns>
    public override string ToString() => Value;
}
