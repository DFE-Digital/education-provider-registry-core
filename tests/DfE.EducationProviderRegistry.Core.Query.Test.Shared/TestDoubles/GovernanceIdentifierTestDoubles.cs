using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles;

public static class GovernanceIdentifierTestDoubles
{
    public static GovernanceIdentifier Create(string value = "1234567")
    {
        return new GovernanceIdentifier(value);
    }
}
