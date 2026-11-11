namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

public enum ValidationErrorCode
{
    LowerBoundBeforeMinimum = 1,
    UpperBoundBeforeMinimum,
    UpperBoundNotAfterLowerBound
}
