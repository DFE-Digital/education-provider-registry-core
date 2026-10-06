namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupStatus
{
    public GroupStatus(string label, DateOnly effectiveDate)
    {
        Label = label;
        EffectiveDate = effectiveDate;
    }

    public string Label { get; }
    public DateOnly EffectiveDate { get; }
}
