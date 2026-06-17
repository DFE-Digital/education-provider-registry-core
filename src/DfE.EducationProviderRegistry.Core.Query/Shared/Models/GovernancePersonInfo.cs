namespace DfE.EducationProviderRegistry.Core.Query.Shared.Models;

public sealed record GovernancePersonInfo(
    string Identifier, // Governance Identifier (7 digit numeric)
    string FullName,
    DateTime StartDate);
