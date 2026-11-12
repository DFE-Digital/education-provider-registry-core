namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

public enum ValidationErrorCode
{
    LowerBeforeMinimum = 1,
    LowerAfterMaximum,
    UpperBeforeMinimum,
    UpperAfterMaximum,
    UpperNotGreaterThanLower,
    LowerNotValid,
    UpperNotValid
}
