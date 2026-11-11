namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

internal sealed class InvalidRangeException : ArgumentException
{
    public InvalidRangeException(IReadOnlyCollection<ValidationErrorCode> errors) : base("One or more range values are invalid.")
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors = errors;
    }

    public IReadOnlyCollection<ValidationErrorCode> Errors { get; }
}
