namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Group
{
    public Group(
        GroupIdentity identity,
        CompaniesHouseId companiesHouseId,
        IEnumerable<Academy>? academies,
        IEnumerable<Member>? members,
        IEnumerable<Trustee>? trustees)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(companiesHouseId);

        GroupId = identity.Id;
        GroupUID = identity.Uid;
        CompaniesHouseId = companiesHouseId;
        Academies = academies?.ToList() ?? [];
        Members = members?.ToList() ?? [];
        Trustees = trustees?.ToList() ?? [];
    }

    public GroupId GroupId { get; }
    public GroupUID GroupUID { get; }
    //public Ukprn Ukprn { get; }
    public CompaniesHouseId CompaniesHouseId { get; }
    //public GroupStatus Status { get; }
    public IReadOnlyCollection<Academy> Academies { get; }
    public IReadOnlyCollection<Member> Members { get; }
    public IReadOnlyCollection<Trustee> Trustees { get; }


    public bool Equals(Group? other)
    {
        if (other is null) return false;

        return GroupId == other.GroupId
            && GroupUID == other.GroupUID
            //&& Ukprn == other.Ukprn
            && CompaniesHouseId == other.CompaniesHouseId
            && Academies.SequenceEqual(other.Academies)
            && Members.SequenceEqual(other.Members)
            && Trustees.SequenceEqual(other.Trustees);
    }


    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(GroupId);
        hash.Add(GroupUID);
        hash.Add(CompaniesHouseId);

        foreach (Academy academy in Academies)
        {
            hash.Add(academy);
        }

        foreach (Member member in Members)
        {
            hash.Add(member);
        }


        foreach (Trustee trustee in Trustees)
        {
            hash.Add(trustee);
        }

        return hash.ToHashCode();
    }

}
