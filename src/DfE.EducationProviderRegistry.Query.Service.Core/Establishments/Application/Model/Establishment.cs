namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

/// <summary>
/// Represents an establishment within the system.
/// This record provides a strongly‑typed wrapper around the
/// <see cref="EstablishmentIdentifier"/> value object and forms
/// the root entity for establishment‑related operations.
/// </summary>
public sealed record Establishment
{
    /// <summary>
    /// Gets the unique identifier for the establishment.
    /// This value is guaranteed to be non‑null and validated
    /// at construction time.
    /// </summary>
    public EstablishmentIdentifier Identifier { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Establishment"/> record
    /// using the specified <paramref name="identifier"/>.
    /// </summary>
    /// <param name="identifier">
    /// The strongly‑typed identifier representing the establishment.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="identifier"/> is <c>null</c>.
    /// </exception>
    public Establishment(EstablishmentIdentifier identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        Identifier = identifier;
    }

    /// <summary>
    /// Creates a new <see cref="Establishment"/> instance using the provided
    /// <paramref name="identifier"/>. This factory method offers a clear,
    /// intention‑revealing way to construct an establishment.
    /// </summary>
    /// <param name="identifier">
    /// The identifier to associate with the new establishment.
    /// </param>
    /// <returns>
    /// A new <see cref="Establishment"/> instance.
    /// </returns>
    public static Establishment Create(EstablishmentIdentifier identifier)
        => new(identifier);
}
