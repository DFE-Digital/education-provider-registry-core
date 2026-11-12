namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

public enum ValidationErrorCode
{
    LowerNotValid = 1,
    LowerBeforeMinimum,
    LowerAfterMaximum,
    UpperNotValid,
    UpperBeforeMinimum,
    UpperAfterMaximum,
    UpperNotGreaterThanLower,
}
