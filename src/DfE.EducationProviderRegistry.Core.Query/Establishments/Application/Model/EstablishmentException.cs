namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

/// <summary>
/// Exception type used for establishment‑related validation or domain errors.
/// </summary>
public sealed class EstablishmentException : ApplicationException
{
    /// <summary>
    /// Creates a new <see cref="EstablishmentException"/> with a message describing the error.
    /// </summary>
    /// <param name="message">A description of the validation or domain failure.</param>
    public EstablishmentException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentException"/> with a message and an inner exception.
    /// </summary>
    /// <param name="message">A description of the validation or domain failure.</param>
    /// <param name="innerException">The underlying exception that caused this error.</param>
    public EstablishmentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
