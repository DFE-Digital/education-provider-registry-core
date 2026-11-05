using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Shared;

public static class GovernanceIdentifierTestDoubles
{
    public static GovernanceIdentifier Create(string value = "1234567")
    {
        return new GovernanceIdentifier(value);
    }
}
