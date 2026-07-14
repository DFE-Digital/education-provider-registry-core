using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;

public sealed class GovernanceIdentifierTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        string? input = "1234567";

        GovernanceIdentifier identifier = new(input);

        Assert.Equal(input, identifier.Value);
    }
}
