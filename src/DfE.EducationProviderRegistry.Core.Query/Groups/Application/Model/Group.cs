namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Group
{
    public Group(
        GroupId id,
        GroupUid uid,
        CompaniesHouseId companiesHouseId,
        IEnumerable<Academy> academies,
        IEnumerable<Member>? members,
        IEnumerable<Trustee>? trustees)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(companiesHouseId);

        GroupId = id;
        GroupUID = uid;
        CompaniesHouseId = companiesHouseId;

        Academies = academies?.ToList() ?? [];
        Members = members?.ToList() ?? [];
        Trustees = trustees?.ToList() ?? [];
    }
    public GroupId GroupId { get; }
    public GroupUid GroupUID { get; }
    public CompaniesHouseId CompaniesHouseId { get; }
    public IReadOnlyCollection<Academy> Academies { get; }
    public IReadOnlyCollection<Member> Members { get; }
    public IReadOnlyCollection<Trustee> Trustees { get; }
}
