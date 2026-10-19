using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupStatusTestDoubles
{
    public static GroupStatus Create(string label = "Closed", DateOnly? effectiveDate = null)
        => new(
            label,
            effectiveDate ?? new DateOnly(2025, 01, 01));
}
