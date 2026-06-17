using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupUidTestDoubles
{
    public static GroupUid Create()
    {
        return new GroupUid(1);
    }

    public static GroupUid Create(int value)
    {
        return new GroupUid(value);
    }
}
