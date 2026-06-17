using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

internal static class UniqueReferenceNumberTestDoubles
{
    public static UniqueReferenceNumber Create(string value = "123456")
    {
        return new UniqueReferenceNumber(value);
    }
}
