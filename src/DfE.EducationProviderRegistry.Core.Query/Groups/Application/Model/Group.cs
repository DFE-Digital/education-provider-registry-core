using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Group
{
    private readonly GroupIdentity _identity;
    private readonly GroupExternalIdentifiers _externalIds;
    private readonly GroupComposition _composition;

    public Group(
        GroupIdentity identity,
        GroupExternalIdentifiers externalIds,
        GroupComposition composition)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(externalIds);
        ArgumentNullException.ThrowIfNull(composition);

        _identity = identity;
        _externalIds = externalIds;
        _composition = composition;

        Ukprn = externalIds.Ukprn ?? Ukprn.CreateNoValue();
        CompaniesHouseId = externalIds.CompaniesHouseId;
    }

    public GroupId GroupId => _identity.Id;
    public GroupUID GroupUID => _identity.Uid;
    public Ukprn Ukprn { get; }
    public CompaniesHouseId? CompaniesHouseId { get; }
    //public GroupStatus Status { get; }
    public IReadOnlyCollection<Academy> Academies => _composition.Academies;
    public IReadOnlyCollection<Member> Members => _composition.Members;
    public IReadOnlyCollection<Trustee> Trustees => _composition.Trustees;


    public bool Equals(Group? other)
    {
        if (other is null)
        {
            return false;
        }

        return GroupId == other.GroupId
            && GroupUID == other.GroupUID
            && Ukprn == other.Ukprn
            && CompaniesHouseId == other.CompaniesHouseId
            && Academies.SequenceEqual(other.Academies)
            && Members.SequenceEqual(other.Members)
            && Trustees.SequenceEqual(other.Trustees);
    }


    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(_identity);
        hash.Add(_externalIds);
        hash.Add(_composition);

        return hash.ToHashCode();
    }

}
