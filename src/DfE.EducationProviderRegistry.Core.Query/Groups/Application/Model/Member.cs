namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Member
{
    public Member(GroupMemberIdentifier id, GroupMemberName name, DateTime startDate)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);

        Id = id;
        Name = name;
        StartDate = startDate;
    }

    public GroupMemberIdentifier Id { get; }
    public GroupMemberName Name { get; }
    public DateTime StartDate { get; }
}
