using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupIdTestDoubles
{
    public static GroupId Stub() => Create("group-1");

    public static GroupId Create(string value)
    {
        return new GroupId(value);
    }
}
