using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;

public sealed class GroupReadModelBuilder
{
    private string _name;
    private string _groupId;
    private long _groupUid;
    private string? _ukprn;
    private string? _companiesHouseId;
    private string? _address;
    private string? _status;
    private string? _type;
    private readonly List<Academy> _academies;
    private readonly List<TrusteeReadModel> _trustees;
    private readonly List<MemberReadModel> _members;

    public GroupReadModelBuilder()
    {
        _name = "Test group";
        _groupId = "group-1";
        _groupUid = 1000L;
        _address = "1 Test Street, TestVille, TE5 1ST";
        _status = "Open";
        _type = "Multi-Academy Trust";

        _academies = [];
        _trustees = [];
        _members = [];
    }
    public static GroupReadModelBuilder Create() => new();

    public GroupReadModelBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public GroupReadModelBuilder WithGroupId(string groupId)
    {
        _groupId = groupId;
        return this;
    }

    public GroupReadModelBuilder WithGroupUid(long uid)
    {
        _groupUid = uid;
        return this;
    }

    public GroupReadModelBuilder WithUkprn(string? ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    public GroupReadModelBuilder WithCompaniesHouseId(string? companiesHouseId)
    {
        _companiesHouseId = companiesHouseId;
        return this;
    }

    public GroupReadModelBuilder WithAddress(string? address)
    {
        _address = address;
        return this;
    }

    public GroupReadModelBuilder WithStatus(string? status)
    {
        _status = status;
        return this;
    }

    public GroupReadModelBuilder WithGroupType(string? type)
    {
        _type = type;
        return this;
    }

    public GroupReadModelBuilder WithAcademy(Academy academy)
    {
        _academies.Add(academy);
        return this;
    }

    public GroupReadModelBuilder WithAcademies(IEnumerable<Academy> academies)
    {
        _academies.AddRange(academies);
        return this;
    }

    public GroupReadModelBuilder WithTrustee(TrusteeReadModel trustee)
    {
        _trustees.Add(trustee);
        return this;
    }

    public GroupReadModelBuilder WithTrustees(IEnumerable<TrusteeReadModel> trustees)
    {
        _trustees.AddRange(trustees);
        return this;
    }

    public GroupReadModelBuilder WithMember(MemberReadModel member)
    {
        _members.Add(member);
        return this;
    }

    public GroupReadModelBuilder WithMembers(IEnumerable<MemberReadModel> members)
    {
        _members.AddRange(members);
        return this;
    }

    public GroupReadModel Build()
    {
        return new()
        {
            Name = _name,
            GroupId = _groupId,
            GroupUID = _groupUid,
            UKPRN = _ukprn!,
            CompaniesHouseId = _companiesHouseId,
            Address = _address!,
            Status = _status!,
            Type = _type!,
            Academies = _academies,
            Trustees = _trustees,
            Members = _members
        };
    }
}
