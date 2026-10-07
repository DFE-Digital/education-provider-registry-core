using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupStatusTestDoubles
{
    internal static GroupStatus Create(string label = "Closed", DateOnly? effectiveDate = null)
        => new(
            label,
            effectiveDate ?? new DateOnly(2025, 01, 01));
}
