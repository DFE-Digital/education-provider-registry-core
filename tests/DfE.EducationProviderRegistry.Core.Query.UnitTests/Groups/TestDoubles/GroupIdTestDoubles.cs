using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupIdTestDoubles
{
    public static GroupId Create() => Create("group-1");

    public static GroupId Create(string value)
    {
        return new GroupId(value);
    }
}
