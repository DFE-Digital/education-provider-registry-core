using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record GovernorModel(GovernanceIdentifier Identifier, Name Name, DateTime StartDate);
