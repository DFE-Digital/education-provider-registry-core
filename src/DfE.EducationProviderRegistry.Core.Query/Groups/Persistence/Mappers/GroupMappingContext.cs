using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;

internal sealed record GroupMappingContext(
    GroupAggregate Group,
    IReadOnlyCollection<GroupEstablishmentAggregate> Establishments);
