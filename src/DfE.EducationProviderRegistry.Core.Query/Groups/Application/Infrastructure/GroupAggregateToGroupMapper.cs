using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using GroupType = DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model.GroupType;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

internal sealed class GroupAggregateToGroupMapper : IMapper<GroupAggregate, Group>
{
    public Group Map(GroupAggregate input)
    {
        ArgumentNullException.ThrowIfNull(input);

        GroupIdentity identity = new(
            id: new(input.GroupId),
            uid: new(input.GroupUid));

        GroupExternalIdentifiers externalIds = new(
          ukprn: new Ukprn(input.Ukprn),
          companiesHouseId: input.CompaniesHouseNumber is null ? null : new CompaniesHouseId(input.CompaniesHouseNumber));

        GroupComposition composition = new(
            academies: [],
            members: [],
            trustees: []
        );

        GroupCharacteristics characteristics = new(
            name: new Name(input.Name),
            address: new SiteAddressModel(
                Name: input.SiteName ?? string.Empty,
                AddressLine1: input.AddressLine1 ?? string.Empty,
                AddressLine2: input.AddressLine2 ?? string.Empty,
                Town: input.Town ?? string.Empty,
                County: input.County ?? string.Empty,
                Postcode: input.Postcode ?? string.Empty),
            type: new GroupType(input.GroupTypeName),
            status:
                input.GroupStatusEffectiveDate is null ?
                    null :
                        new GroupStatus(
                            label: input.GroupStatusLabel ?? string.Empty,
                            effectiveDate: input.GroupStatusEffectiveDate.Value)
        );

        return new Group(identity, externalIds, composition, characteristics);

    }
}
