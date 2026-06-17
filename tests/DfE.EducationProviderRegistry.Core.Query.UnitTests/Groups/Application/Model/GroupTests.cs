using System;
using System.Collections.Generic;
using System.Linq;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using Xunit;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

using System;
using System.Collections.Generic;
using Xunit;

public sealed class GroupTests
{
    // ----------------------------
    // Guard clauses
    // ----------------------------

    [Fact]
    public void Constructor_WhenGroupIdIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Group> construct = () => CreateSut(
            null,
            GroupUidTestDoubles.Create(),
            CompaniesHouseIdTestDoubles.Create(),
            NoAcademies(),
            MemberTestDoubles.Empty(),
            TrusteeTestDoubles.Empty());

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenCompaniesHouseIdIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Group> construct = () => CreateSut(
            GroupIdTestDoubles.Create(),
            GroupUidTestDoubles.Create(),
            null,
            [],
            [],
            []);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenValidArguments_SetsProperties()
    {
        // Arrange
        GroupId id = GroupIdTestDoubles.Create();
        GroupUid uid = GroupUidTestDoubles.Create();
        CompaniesHouseId companiesHouseId = CompaniesHouseIdTestDoubles.Create();
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(2);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(2);

        // Act
        Group result = CreateValidSut(
            groupId: id,
            groupUid: uid,
            companiesHouseId: companiesHouseId,
            members: members,
            trustees: trustees);

        // Assert
        Assert.Equal(id, result.GroupId);
        Assert.Equal(uid, result.GroupUID);
        Assert.Equal(companiesHouseId, result.CompaniesHouseId);

        Assert.Equal(members.Count, result.Members.Count);
        Assert.Equal(trustees.Count, result.Trustees.Count);
    }

    [Fact]
    public void Constructor_WhenOptionalCollectionsAreNull_SetsEmptyCollections()
    {
        // Arrange

        // Act
        Group result = CreateValidSut(
            members: null,
            trustees: null);

        // Assert
        Assert.Empty(result.Members);
        Assert.Empty(result.Trustees);
    }

    [Fact]
    public void Constructor_WhenAcademiesIsNull_SetsEmptyCollection()
    {
        // Arrange

        // Act
        Group result = CreateValidSut(academies: null);

        // Assert
        Assert.Empty(result.Academies);
    }

    [Fact]
    public void Equality_WhenAllPropertiesEqual_ShouldBeEqual()
    {
        // Arrange
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(1);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(1);

        Group left = CreateValidSut(members: members, trustees: trustees);
        Group right = CreateValidSut(members: members, trustees: trustees);

        // Act & Assert
        Assert.Equal(left, right);
    }

    [Fact]
    public void Equality_WhenCollectionsAreDifferentInstances_ShouldNotBeEqual()
    {
        // Arrange
        Group left = CreateValidSut(members: MemberTestDoubles.Create(1));
        Group right = CreateValidSut(members: MemberTestDoubles.Create(1));

        // Act & Assert
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Equality_WhenGroupIdDiffers_ShouldNotBeEqual()
    {
        // Arrange
        Group left = CreateValidSut(groupId: GroupIdTestDoubles.Create("group-1"));
        Group right = CreateValidSut(groupId: GroupIdTestDoubles.Create("group-2"));

        // Act & Assert
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void GetHashCode_WhenEqual_ShouldReturnSameValue()
    {
        // Arrange
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(1);

        Group left = CreateValidSut(members: members);
        Group right = CreateValidSut(members: members);

        // Act
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    private static Group CreateSut(
        GroupId groupId,
        GroupUid groupUid,
        CompaniesHouseId companiesHouseId,
        IReadOnlyCollection<Academy> academies,
        IReadOnlyCollection<Member> members,
        IReadOnlyCollection<Trustee> trustees)
    {
        return new Group(
            groupId,
            groupUid,
            companiesHouseId,
            academies,
            members,
            trustees);
    }

    private static Group CreateValidSut(
        GroupId? groupId = null,
        GroupUid? groupUid = null,
        CompaniesHouseId? companiesHouseId = null,
        IReadOnlyCollection<Academy>? academies = null,
        IReadOnlyCollection<Member>? members = null,
        IReadOnlyCollection<Trustee>? trustees = null)
    {
        return CreateSut(
            groupId ?? GroupIdTestDoubles.Create(),
            groupUid ?? GroupUidTestDoubles.Create(),
            companiesHouseId ?? CompaniesHouseIdTestDoubles.Create(),
            academies ?? NoAcademies(),
            members ?? [],
            trustees ?? []);
    }
}



internal static class GroupIdTestDoubles
{
    public static GroupId Create()
    {
        return new GroupId("group-1");
    }

    public static GroupId Create(string value)
    {
        return new GroupId(value);
    }
}

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

internal static class CompaniesHouseIdTestDoubles
{
    public static CompaniesHouseId Create()
    {
        return new CompaniesHouseId("CH123");
    }

    public static CompaniesHouseId Create(string value)
    {
        return new CompaniesHouseId(value);
    }
}
