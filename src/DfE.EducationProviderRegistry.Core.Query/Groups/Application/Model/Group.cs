namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Group
{
    public Group(
        GroupIdentifier id,
        GroupUniqueIdentifier uid,
        CompaniesHouseIdentifier companiesHouseId,
        IEnumerable<Member>? members,
        IEnumerable<Trustee>? trustees)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(companiesHouseId);

        GroupId = id;
        GroupUID = uid;
        CompaniesHouseId = companiesHouseId;
        Members = members?.ToList() ?? [];
        Trustees = trustees?.ToList() ?? [];
    }
    public GroupIdentifier GroupId { get; }
    public GroupUniqueIdentifier GroupUID { get; }
    public CompaniesHouseIdentifier CompaniesHouseId { get; }
    public IReadOnlyCollection<Member> Members { get; }
    public IReadOnlyCollection<Trustee> Trustees { get; }
}
