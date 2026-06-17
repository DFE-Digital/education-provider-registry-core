namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

/// <summary>
/// Represents a validated field name that can be used for sorting search results.
/// Ensures the field is included in the configured list of allowed sort fields.
/// </summary>
public sealed class SortField
{
    /// <summary>
    /// Gets the validated field name to sort by.
    /// This value is guaranteed to be one of the allowed fields and retains its original casing.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Internal set of allowed field names for sorting.
    /// Uses case-insensitive comparison for validation and is immutable after construction.
    /// </summary>
    private readonly HashSet<string> _validSortFields;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortField"/> class
    /// after validating the provided field name against the allowed field list.
    /// </summary>
    /// <param name="sortField">The field name to sort by (e.g., "Surname", "DOB").</param>
    /// <param name="validSortFields">The list of allowed field names for sorting.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="sortField"/> is null or empty.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="validSortFields"/> is null, empty, contains duplicates,
    /// or does not include the provided <paramref name="sortField"/>.
    /// </exception>
    public SortField(string sortField, IReadOnlyList<string> validSortFields)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(sortField);

        if (validSortFields == null || validSortFields.Count == 0)
        {
            throw new ArgumentException(
                "Valid sort fields list cannot be null or empty.", nameof(validSortFields));
        }

        if (HasDuplicates(validSortFields))
        {
            throw new ArgumentException(
                "Valid sort fields list contains duplicate entries (case-insensitive).", nameof(validSortFields));
        }

        _validSortFields =
            new HashSet<string>(
                validSortFields, StringComparer.OrdinalIgnoreCase);

        if (!IsValid(sortField))
        {
            throw new ArgumentException(
                $"Unknown sort field '{sortField}'", nameof(sortField));
        }

        Field = sortField;
    }

    /// <summary>
    /// Determines whether the provided field name is valid for sorting.
    /// Comparison is case-insensitive.
    /// </summary>
    /// <param name="field">The field name to validate.</param>
    /// <returns>
    /// <c>true</c> if the field is included in the allowed list; otherwise <c>false</c>.
    /// </returns>
    public bool IsValid(string field) => _validSortFields.Contains(field);

    /// <summary>
    /// Gets a read-only view of the valid sort fields.
    /// Useful for diagnostics, UI display, or API metadata.
    /// </summary>
    public IReadOnlyCollection<string> ValidFields => _validSortFields;

    /// <summary>
    /// Determines whether the provided list contains duplicate entries
    /// using case-insensitive comparison.
    /// </summary>
    /// <param name="fields">The list of field names to inspect.</param>
    /// <returns>
    /// <c>true</c> if duplicates are found; otherwise <c>false</c>.
    /// </returns>
    private static bool HasDuplicates(IReadOnlyList<string> fields)
    {
        HashSet<string> duplicates =
            new(StringComparer.OrdinalIgnoreCase);

        return fields.Any(field => !duplicates.Add(field));
    }

    /// <summary>
    /// Creates a new validated <see cref="SortField"/> instance.
    /// </summary>
    /// <param name="field">The field name to sort by.</param>
    /// <param name="validFields">The list of allowed field names.</param>
    /// <returns>
    /// A validated <see cref="SortField"/> instance.
    /// </returns>
    public static SortField Create(
        string field, IReadOnlyList<string> validFields) => new(field, validFields);
}
