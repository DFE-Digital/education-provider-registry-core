using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal sealed class GroupBuilder
{
    private string _groupId = "1234567";
    private int _groupUid = 1234;
    private string _ukprn = "UKPRN-1";
    private string _companiesHouseId = "CH1";

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
                trustees: _trustees));
    }
}
