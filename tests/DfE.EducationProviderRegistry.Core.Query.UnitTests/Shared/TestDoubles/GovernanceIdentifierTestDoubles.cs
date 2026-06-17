using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

internal static class GovernanceIdentifierTestDoubles
{
    public static GovernanceIdentifier Create(string value = "1234567")
    {
        return new GovernanceIdentifier(value);
    }
}
