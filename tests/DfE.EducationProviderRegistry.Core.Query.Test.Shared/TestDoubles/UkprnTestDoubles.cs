using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles;

public static class UkprnTestDoubles
{
    public static Ukprn Stub() => Create("ukprn-1");
    public static Ukprn Create(string value) => new(value);
}
