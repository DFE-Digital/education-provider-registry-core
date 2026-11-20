using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class GroupStatusTestDoubles
{
    public static GroupStatus Create(
        string label = "Closed",
        DateOnly? effectiveDate = null,
        DateOnly? openedDate = null,
        DateOnly? closedDate = null)
        => new(
            label,
            effectiveDate ?? new DateOnly(2025, 01, 01),
            openedDate,
            closedDate);
}
