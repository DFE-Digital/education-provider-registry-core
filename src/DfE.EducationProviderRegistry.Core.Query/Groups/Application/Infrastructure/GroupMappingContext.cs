using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

internal sealed record GroupMappingContext(
    GroupAggregate Group,
    IReadOnlyCollection<GroupEstablishmentAggregate> Establishments);
