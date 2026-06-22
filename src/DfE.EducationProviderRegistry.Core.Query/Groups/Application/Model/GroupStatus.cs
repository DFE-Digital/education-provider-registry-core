namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupStatus
{
    public GroupStatus(GroupOpenState state, DateTime effectiveDate)
    {
        State = state;
        EffectiveDate = effectiveDate;
    }

    public GroupOpenState State { get; }
    public DateTime EffectiveDate { get; }
}
