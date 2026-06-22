using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Group
{
    private readonly GroupIdentity _identity;
    private readonly GroupExternalIdentifiers _externalIds;
    private readonly GroupComposition _composition;
    private readonly GroupCharacteristics _characteristics;

    public Group(
        GroupIdentity identity,
        GroupExternalIdentifiers externalIds,
        GroupComposition composition,
        GroupCharacteristics characteristics)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(externalIds);
        ArgumentNullException.ThrowIfNull(composition);
        ArgumentNullException.ThrowIfNull(characteristics);

        _identity = identity;
        _externalIds = externalIds;
        _composition = composition;
        _characteristics = characteristics;

        Ukprn = externalIds.Ukprn ?? Ukprn.CreateNoValue();
        CompaniesHouseId = externalIds.CompaniesHouseId;

        Name = characteristics.Name;
        Address = characteristics.Address;
        GroupType = characteristics.Type;
        Status = characteristics.Status;
    }

    public GroupId GroupId => _identity.Id;
    public GroupUID GroupUID => _identity.Uid;
    public Ukprn Ukprn { get; }
    public CompaniesHouseId? CompaniesHouseId { get; }
    public Name Name { get; }
    public Address Address { get; }
    public GroupType GroupType { get; }
    public GroupStatus Status { get; }
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
            && Address == other.Address
            && GroupType == other.GroupType
            && Status == other.Status
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
        hash.Add(_characteristics);

        return hash.ToHashCode();
    }

}
