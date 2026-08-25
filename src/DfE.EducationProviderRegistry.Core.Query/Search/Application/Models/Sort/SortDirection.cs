namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

/// <summary>
/// Represents a validated and normalized sort direction used when ordering
/// search results. Ensures consistency with the expected values required by
/// the underlying search provider.
/// </summary>
public sealed record SortDirection
{
    public static readonly SortDirection Ascending = new(AscendingSort);

    public static readonly SortDirection Descending = new(DescendingSort);

    /// <summary>
    /// Constant representing descending sort direction.
    /// </summary>
    private const string DescendingSort = "desc";

    /// <summary>
    /// Constant representing ascending sort direction.
    /// </summary>
    private const string AscendingSort = "asc";

    /// <summary>
    /// Gets the normalized and validated sort direction string.
    /// Always stored in lowercase (<c>"asc"</c> or <c>"desc"</c>) for compatibility
    /// with Azure Search and other search providers.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortDirection"/> class
    /// after validating and normalizing the provided direction string.
    /// </summary>
    /// <param name="direction">
    /// The sort direction string provided by the caller. Accepts any casing
    /// (e.g., "ASC", "Desc") and normalizes to lowercase.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="direction"/> is null, empty, or not one of the
    /// accepted values (<c>"asc"</c> or <c>"desc"</c>).
    /// </exception>
    public SortDirection(string direction)
    {
        ArgumentException.ThrowIfNullOrEmpty(direction);

        // Normalize casing to lowercase using invariant culture for consistency across locales.
        string normalizedSortDirection = direction.ToLowerInvariant();

        // Validate against known sort directions.
        if (!IsValid(normalizedSortDirection))
        {
            throw new ArgumentException(
                $"Unknown sort direction '{normalizedSortDirection}'", nameof(direction));
        }

        // Store the normalized direction
        Value = normalizedSortDirection;
    }

    /// <summary>
    /// Determines whether the provided direction string is valid.
    /// </summary>
    /// <param name="direction">
    /// The normalized direction string to validate.
    /// </param>
    /// <returns>
    /// <c>true</c> if the direction is <c>"asc"</c> or <c>"desc"</c>;
    /// otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValid(string? direction) =>
        direction is not null &&
            (direction.Equals(DescendingSort) || direction.Equals(AscendingSort));

    /// <summary>
    /// Creates a new validated <see cref="SortDirection"/> instance.
    /// </summary>
    /// <param name="direction">
    /// The sort direction string to validate and normalize.
    /// </param>
    /// <returns>
    /// A validated <see cref="SortDirection"/> instance.
    /// </returns>
    public static SortDirection Create(string direction) => new(direction);
}
