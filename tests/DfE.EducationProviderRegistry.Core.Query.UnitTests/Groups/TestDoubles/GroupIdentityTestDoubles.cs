using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupIdentityTestDoubles
{
    internal static GroupIdentity Create(GroupId? id = null, GroupUID? uid = null)
    {
        return new(
            id ?? GroupIdTestDoubles.Create(),
            uid: uid ?? GroupUIDTestDoubles.Create());
    }
}
