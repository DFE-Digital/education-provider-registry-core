using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupCompositionTestDoubles
{
    public static GroupComposition Create(
        IEnumerable<Academy>? academies = null,
        IEnumerable<Member>? members = null,
        IEnumerable<Trustee>? trustees = null)
    {
        return new GroupComposition(
            academies ?? [],
            members ?? [],
            trustees ?? []);
    }
}
