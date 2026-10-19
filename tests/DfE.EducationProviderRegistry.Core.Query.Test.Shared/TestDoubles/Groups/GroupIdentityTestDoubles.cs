using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupIdentityTestDoubles
{
    public static GroupIdentity Stub()
    {
        return new(id:
            GroupIdTestDoubles.Stub(),
            GroupUIDTestDoubles.Stub());
    }

    public static GroupIdentity Create(GroupId? id = null, GroupUID? uid = null)
    {
        return new(
            id ?? GroupIdTestDoubles.Stub(),
            uid: uid ?? GroupUIDTestDoubles.Create());
    }
}
