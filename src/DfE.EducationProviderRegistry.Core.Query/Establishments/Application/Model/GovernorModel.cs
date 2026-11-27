using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record GovernorModel(
    GovernanceIdentifier Identifier,
    string? Title,
    string? Firstname,
    string Lastname,
    string Role,
    int RoleOrder,
    string? AppointedBy,
    DateOnly StartDate,
    DateOnly? EndDate);
