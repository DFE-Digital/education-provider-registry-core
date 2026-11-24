using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class GroupCompositionTestDoubles
{
    public static GroupComposition Create(
        IEnumerable<Member>? members = null,
        IEnumerable<Trustee>? trustees = null,
        IEnumerable<Establishment>? establishments = null)
    {
        return new GroupComposition(
            members ?? [],
            trustees ?? [],
            establishments ?? []);
    }
}
