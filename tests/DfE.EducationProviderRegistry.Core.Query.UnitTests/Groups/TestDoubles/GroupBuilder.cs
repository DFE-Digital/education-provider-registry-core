using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal sealed class GroupBuilder
{
    private string _groupId = "1234567";
    private int _groupUid = 1234;
    private string _ukprn = "UKPRN-1";
    private string _companiesHouseId = "CH1";
    private GroupType _type = GroupTypeTestDoubles.Create();
    private Address _address = AddressTestDoubles.Generate();
    private GroupStatus _status = new(GroupOpenState.Open, new(2025, 01, 02));

    private IEnumerable<Academy> _academies = [];
    private IEnumerable<Member>? _members = [];
    private IEnumerable<Trustee>? _trustees = [];


    public GroupBuilder WithGroupId(string value)
    {
        _groupId = value;
        return this;
    }

    public GroupBuilder WithGroupUid(int value)
    {
        _groupUid = value;
        return this;
    }

    public GroupBuilder WithUkprn(string value)
    {
        _ukprn = value;
        return this;
    }

    public GroupBuilder WithCompaniesHouseId(string value)
    {
        _companiesHouseId = value;
        return this;
    }

    public GroupBuilder WithGroupStatus(GroupOpenState state, DateTime effectiveFrom)
    {
        _status = new(state, effectiveFrom);
        return this;
    }

    public GroupBuilder WithAddress(string? street = null, string? town = null, string? county = null, string? postcode = null)
    {
        _address = new Address(street!, town!, county!, postcode!);
        return this;
    }

    public GroupBuilder WithType(string type)
    {
        _type = new(type);
        return this;
    }

    public GroupBuilder WithAcademies(IEnumerable<Academy> academies)
    {
        _academies = academies;
        return this;
    }

    public GroupBuilder WithMembers(IEnumerable<Member>? members)
    {
        _members = members;
        return this;
    }

    public GroupBuilder WithTrustees(IEnumerable<Trustee>? trustees)
    {
        _trustees = trustees;
        return this;
    }

    public Group Build()
    {
        return new Group(
            identity:
                new GroupIdentity(
                    new GroupId(_groupId),
                    new GroupUID(_groupUid)),
            externalIds: new GroupExternalIdentifiers(
                new Ukprn(_ukprn),
                new CompaniesHouseId(_companiesHouseId)),
            composition: new GroupComposition(
                academies: _academies,
                members: _members,
                trustees: _trustees),
            characteristics: new GroupCharacteristics(
                _address,
                _type,
                _status));
    }
}
