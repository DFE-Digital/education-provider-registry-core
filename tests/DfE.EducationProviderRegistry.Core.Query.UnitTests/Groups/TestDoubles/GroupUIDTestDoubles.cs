using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupUIDTestDoubles
{
    public static GroupUID Create() => Create(1);

    public static GroupUID Create(int value)
    {
        return new GroupUID(value);
    }
}
