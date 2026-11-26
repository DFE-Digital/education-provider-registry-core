using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Trustee
{
    public Trustee(
        GovernanceIdentifier id,
        Name name,
        string role,
        DateOnly startDate,
        DateOnly? endDate = null,
        string? title = null,
        string? appointedBy = null)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        Id = id;
        Name = name;
        Role = role;
        StartDate = startDate;
        EndDate = endDate;
        Title = title;
        AppointedBy = appointedBy;
    }

    public GovernanceIdentifier Id { get; }
    public Name Name { get; }
    public string? Title { get; }
    public string Role { get; }
    public string? AppointedBy { get; }
    public DateOnly StartDate { get; }
    public DateOnly? EndDate { get; }
}
