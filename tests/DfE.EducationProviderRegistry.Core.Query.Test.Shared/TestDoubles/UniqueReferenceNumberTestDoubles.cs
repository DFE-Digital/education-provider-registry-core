using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles;

public static class UniqueReferenceNumberTestDoubles
{
    public static UniqueReferenceNumber Stub()
    {
        return new("123456");
    }

    public static UniqueReferenceNumber Create(string value)
    {
        return new UniqueReferenceNumber(value);
    }

    public static UniqueReferenceNumber Random()
    {
        return new UniqueReferenceNumber(
            new Random().Next(
                minValue: 100000,
                maxValue: 999999).ToString());
    }
}
