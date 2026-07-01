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
            id: new GroupId(
                input.GroupIdentifier
                    .Where(i => i.IdentifierType == "GROUPID")
                    .Select(i => i.IdentifierValue)
                    .SingleOrDefault()!
            ),
            uid: new(input.GroupId)
        );

        GroupExternalIdentifiers externalIds = new(
            ukprn: new Ukprn(
                input.GroupIdentifier
                    .Where(i => i.IdentifierType == "UKPRN")
                    .Select(i => i.IdentifierValue)
                    .SingleOrDefault()!
            ),
            companiesHouseId: input.GroupIdentifier
                .Where((groupIdentifier) => groupIdentifier.IdentifierType == "COMPANIESHOUSE")
                .Select((companiesHouseIdentifier) => companiesHouseIdentifier.IdentifierValue)
                .SingleOrDefault() is string chn
                    ? new CompaniesHouseId(chn)
                    : null
        );

        GroupComposition composition = new(
            academies: input.EstablishmentGroupMembership
                .Select((estabGroupMembership) => new Academy(
                    new AcademyId(
                        new UniqueReferenceNumber(estabGroupMembership.Establishment.Urn)),
                    new AcademyName(estabGroupMembership.Establishment.Name)))
                .ToList(),

            members: input.RoleAssignment
                .Where((roleAssignment) => roleAssignment.Role.RoleType.Code == "MEMBER")
                .Select((memberRoleAssignment) => new Member(
                    new GovernanceIdentifier(memberRoleAssignment.RoleId.ToString()),
                    new Name(memberRoleAssignment.Role.Person.DisplayName),
                    startDate: DateTime.UtcNow // placeholder until schema updated
                ))
                .ToList(),

            trustees: input.RoleAssignment
                .Where(roleAssignment => roleAssignment.Role.RoleType.Code == "TRUSTEE")
                .Select(trusteeRoleAssignment => new Trustee(
                    id: null,
                    name: new Name(trusteeRoleAssignment.Role.Person.DisplayName),
                    startDate: DateTime.UtcNow, // placeholder
                    title: null
                ))
                .ToList()
        );

        GroupCharacteristics characteristics = new(
            name: new Name(input.Name),
            address: new Address("123 Test Street", "Testville", "Testshire", "TE5 5ST"),   // placeholder - not yet supported in schema
            type: new GroupType(input.GroupType.Name),
            status: new GroupStatus(GroupOpenState.Open, new(2026, 01, 01))     // placeholder - not yet supported in schema
        );

        return new Group(identity, externalIds, composition, characteristics);

    }
}
