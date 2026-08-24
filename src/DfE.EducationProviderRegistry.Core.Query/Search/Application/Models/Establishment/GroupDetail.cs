namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

/// <summary>
/// Represents group‑level information associated with an establishment,
/// such as the trust or federation it belongs to. This value object provides
/// a strongly typed representation of both the group's display name and its
/// identifying code.
/// </summary>
public sealed record class GroupDetail
{
    /// <summary>
    /// Gets the display name of the group the establishment is part of.
    /// </summary>
    public string PartOfName { get; }

    /// <summary>
    /// Gets the identifying code of the group the establishment is part of.
    /// </summary>
    public string PartOfCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupDetail"/> record.
    /// </summary>
    /// <param name="partOfName">The display name of the group.</param>
    /// <param name="partOfCode">The identifying code of the group.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="partOfName"/> or <paramref name="partOfCode"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="partOfName"/> or <paramref name="partOfCode"/> is empty or whitespace.
    /// </exception>
    public GroupDetail(string partOfName, string partOfCode)
    {
        ArgumentNullException.ThrowIfNull(partOfName);
        ArgumentNullException.ThrowIfNull(partOfCode);

        PartOfName = partOfName;
        PartOfCode = partOfCode;
    }

    /// <summary>
    /// Creates a new <see cref="GroupDetail"/> instance. This factory method
    /// provides an intention‑revealing alternative to directly invoking the constructor.
    /// </summary>
    /// <param name="partOfName">The display name of the group.</param>
    /// <param name="partOfCode">The identifying code of the group.</param>
    /// <returns>A fully validated <see cref="GroupDetail"/> instance.</returns>
    public static GroupDetail Create(
        string partOfName,
        string partOfCode) =>
            new(partOfName, partOfCode);
}
