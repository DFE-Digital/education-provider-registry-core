using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupStatusTestDoubles
{
    internal static GroupStatus Create() => Create(GroupOpenState.Closed);
    internal static GroupStatus Create(GroupOpenState state, DateTime? effectiveDate = null)
        => new(
            state,
            effectiveDate ?? new DateTime(2025, 01, 01));
}
