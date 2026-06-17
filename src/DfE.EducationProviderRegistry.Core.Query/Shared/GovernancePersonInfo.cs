namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record GovernancePersonInfo(
    GovernanceIdentifier Identifier,
    Name FullName,
    DateTime StartDate);
