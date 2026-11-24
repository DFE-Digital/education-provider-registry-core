using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupIdentityTests
{
    [Fact]
    public void Constructor_Sets_Properties_When_Valid()
    {
        GroupId id = GroupIdTestDoubles.Stub();
        GroupUID uid = GroupUIDTestDoubles.Create(1);

        GroupIdentity identity = new(uid, id);

        Assert.Same(id, identity.Id);
        Assert.Equal(uid, identity.Uid);
    }

    [Fact]
    public void Equivalent_When_Properties_Same()
    {
        GroupIdentity left = new(
            GroupUIDTestDoubles.Create(1),
            GroupIdTestDoubles.Create("group-1"));

        GroupIdentity right = new(
            GroupUIDTestDoubles.Create(1),
            GroupIdTestDoubles.Create("group-1"));

        Assert.Equal(left, right);
    }

    [Fact]
    public void NotEquivalent_When_GroupId_Different()
    {
        GroupIdentity left = new(
            GroupUIDTestDoubles.Create(1),
            GroupIdTestDoubles.Create("group-1"));

        GroupIdentity right = new(
            GroupUIDTestDoubles.Create(1),
            GroupIdTestDoubles.Create("group-2"));

        Assert.NotEqual(left, right);
    }

    [Fact]
    public void NotEquivalent_When_GroupUID_Different()
    {
        GroupIdentity left = new(
            GroupUIDTestDoubles.Create(1),
            GroupIdTestDoubles.Create("group-1"));

        GroupIdentity right = new(
            GroupUIDTestDoubles.Create(999),
            GroupIdTestDoubles.Create("group-1"));

        Assert.NotEqual(left, right);
    }
}
