using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupIdentityTests
{
    [Fact]
    public void Constructor_Throws_When_GroupId_Is_Null()
    {
        Func<GroupIdentity> construct =
            () => new(
                id: null!, uid: GroupUIDTestDoubles.Create(1));

        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Sets_Properties_When_Valid()
    {
        GroupId id = GroupIdTestDoubles.Create();
        GroupUID uid = GroupUIDTestDoubles.Create(1);

        GroupIdentity identity = new(id, uid);

        Assert.Same(id, identity.Id);
        Assert.Equal(uid, identity.Uid);
    }

    [Fact]
    public void Equivalent_When_Properties_Same()
    {
        GroupIdentity left = new(
            GroupIdTestDoubles.Create("group-1"),
            GroupUIDTestDoubles.Create(1));

        GroupIdentity right = new(
            GroupIdTestDoubles.Create("group-1"),
            GroupUIDTestDoubles.Create(1));

        Assert.Equal(left, right);
    }

    [Fact]
    public void NotEquivalent_When_GroupId_Different()
    {
        GroupIdentity left = new(
            GroupIdTestDoubles.Create("group-1"),
            GroupUIDTestDoubles.Create(1));

        GroupIdentity right = new(
            GroupIdTestDoubles.Create("group-2"),
            GroupUIDTestDoubles.Create(1));

        Assert.NotEqual(left, right);
    }

    [Fact]
    public void NotEquivalent_When_GroupUID_Different()
    {
        GroupIdentity left = new(
            GroupIdTestDoubles.Create("group-1"),
            GroupUIDTestDoubles.Create(1));

        GroupIdentity right = new(
            GroupIdTestDoubles.Create("group-1"),
            GroupUIDTestDoubles.Create(999));

        Assert.NotEqual(left, right);
    }
}
