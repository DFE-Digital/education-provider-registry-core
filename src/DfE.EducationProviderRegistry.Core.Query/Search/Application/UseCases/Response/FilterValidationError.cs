using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

/// <summary>
/// Represents a validation error associated with a filter.
/// </summary>
public sealed class FilterValidationError
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filterName"></param>
    /// <param name="errorMessage"></param>
    public FilterValidationError(string filterName, ValidationErrorCode errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filterName);

        FilterName = filterName;
        ErrorCode = errorMessage;
    }

    public string FilterName { get; }
    public ValidationErrorCode ErrorCode { get; }
}
