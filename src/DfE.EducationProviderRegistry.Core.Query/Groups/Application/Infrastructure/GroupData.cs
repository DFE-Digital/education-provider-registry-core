using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

internal sealed record GroupData(
    GroupRecord Group,
    IReadOnlyCollection<GroupIdentifier> Identifiers,
    IReadOnlyCollection<EstablishmentGroupMembership> EstabGroupMemberships,
    Contact? Contact);
