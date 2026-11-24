using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using GroupType = DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model.GroupType;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

internal sealed class GroupMappingContextToGroupMapper : IMapper<GroupMappingContext, Group>
{
    private readonly IMapper<IEnumerable<GroupEstablishmentAggregate>, IReadOnlyCollection<Establishment>> _establishmentMapper;

    public GroupMappingContextToGroupMapper(IMapper<IEnumerable<GroupEstablishmentAggregate>, IReadOnlyCollection<Establishment>> establishmentMapper)
    {
        ArgumentNullException.ThrowIfNull(establishmentMapper);
        _establishmentMapper = establishmentMapper;
    }

    public Group Map(GroupMappingContext input)
    {
        ArgumentNullException.ThrowIfNull(input);

        GroupIdentity identity = new(
            uid: new(input.Group.GroupUid),
            id: input.Group.GroupId is null ? null : new(input.Group.GroupId));

        GroupExternalIdentifiers externalIds = new(
          ukprn: input.Group.Ukprn is null ? null : new Ukprn(input.Group.Ukprn),
          companiesHouseId: input.Group.CompaniesHouseNumber is null ? null : new CompaniesHouseId(input.Group.CompaniesHouseNumber));

        GroupComposition composition = new(
            academies: [],
            members: [],
            trustees: [],
            establishments: _establishmentMapper.Map(input.Establishments)
        );

        GroupCharacteristics characteristics = new(
            name: new Name(input.Group.Name),
            address: new SiteAddressModel(
                Name: input.Group.SiteName ?? string.Empty,
                AddressLine1: input.Group.AddressLine1 ?? string.Empty,
                AddressLine2: input.Group.AddressLine2 ?? string.Empty,
                Town: input.Group.Town ?? string.Empty,
                County: input.Group.County ?? string.Empty,
                Postcode: input.Group.Postcode ?? string.Empty),
            type: new GroupType(input.Group.GroupTypeName),
            status:
                input.Group.GroupStatusEffectiveDate is null ?
                    null :
                        new GroupStatus(
                            label: input.Group.GroupStatusLabel ?? string.Empty,
                            effectiveDate: input.Group.GroupStatusEffectiveDate.Value,
                            openedDate: input.Group.OpenedDate,
                            closedDate: input.Group.ClosedDate)
        );

        return new Group(identity, externalIds, composition, characteristics);

    }
}
