using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Member
{
    public Member(GovernanceIdentifier id, Name name, DateTime startDate)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);

        Id = id;
        Name = name;
        StartDate = startDate;
    }

    public GovernanceIdentifier Id { get; }
    public Name Name { get; }
    public DateTime StartDate { get; }
}
