using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

internal static class NameTestDoubles
{
    public static Name Create(string value = "Test Name") => new(value);
}
