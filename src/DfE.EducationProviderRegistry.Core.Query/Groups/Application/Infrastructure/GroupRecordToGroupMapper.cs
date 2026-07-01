using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using GroupType = DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model.GroupType;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

internal sealed class GroupRecordToGroupMapper : IMapper<GroupRecord, Group>
{
    public Group Map(GroupRecord input)
    {
        ArgumentNullException.ThrowIfNull(input);

        GroupIdentity identity = new(
            id: new GroupId("STUB-GROUPID"),
            uid: new(input.GroupId));

        GroupExternalIdentifiers externalIds = new(
          ukprn: new Ukprn("STUB-UKPRN"),
          companiesHouseId: new CompaniesHouseId("STUB-COMPANIESHOUSE-NUMBER"));

        GroupComposition composition = new(
            academies: [],
            members: [],
            trustees: []
        );

        GroupCharacteristics characteristics = new(
            name: new Name(input.Name),
            address: new Address("123 Test Street", "Testville", "Testshire", "TE5 5ST"), // STUB
            type: new GroupType(input.GroupType.Name),
            status: new GroupStatus(GroupOpenState.Open, new(2026, 01, 01)) // STUB
        );

        return new Group(identity, externalIds, composition, characteristics);

    }
}
