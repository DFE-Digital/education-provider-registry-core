using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class UkprnTestDoubles
{
    internal static Ukprn Create() => Create("ukprn-1");
    internal static Ukprn Create(string value) => new(value);
}
