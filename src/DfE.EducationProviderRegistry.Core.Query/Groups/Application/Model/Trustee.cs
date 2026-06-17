using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Trustee
{
    public Trustee(GroupMemberIdentifier id, Name name, DateTime startDate, TrusteeTitle? title = null)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);

        Id = id;
        Name = name;
        StartDate = startDate;
        Title = title;
    }
    public GroupMemberIdentifier Id { get; }
    public TrusteeTitle? Title { get; }
    public Name Name { get; }
    public DateTime StartDate { get; }
}
