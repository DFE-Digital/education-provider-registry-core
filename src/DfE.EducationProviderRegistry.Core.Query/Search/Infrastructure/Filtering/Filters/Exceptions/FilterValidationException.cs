using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

public sealed class FilterValidationException : Exception
{
    public FilterValidationException(IReadOnlyCollection<FilterValidationError> errors) : base("One or more search filters are invalid.")
    {
        ArgumentNullException.ThrowIfNull(errors);
        ValidationErrors = errors;
    }

    public IReadOnlyCollection<FilterValidationError> ValidationErrors { get; }
}
