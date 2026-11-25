namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupStatus
{
    public GroupStatus(
        string label,
        DateOnly? effectiveDate,
        DateOnly? openedDate,
        DateOnly? closedDate)
    {
        Label = label;
        EffectiveDate = effectiveDate;
        OpenedDate = openedDate;
        ClosedDate = closedDate;
    }

    public string Label { get; }
    public DateOnly? EffectiveDate { get; }
    public DateOnly? OpenedDate { get; }
    public DateOnly? ClosedDate { get; }
}
