namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Represents errors that occur during execution of a search operation
/// within the Education Provider Registry search domain.
/// </summary>
public sealed class SearchException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchException"/> class
    /// using the specified error message.
    /// </summary>
    /// <param name="message">
    /// A descriptive message explaining the reason for the exception.
    /// </param>
    public SearchException(string message)
        : base(message){
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchException"/> class
    /// using the specified error message and a reference to the underlying
    /// exception that caused this error.
    /// </summary>
    /// <param name="message">
    /// A descriptive message explaining the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that triggered this error, providing additional context.
    /// </param>
    public SearchException(string message, Exception innerException)
        : base(message, innerException){
    }
}
