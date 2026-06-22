using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupTests
{
    [Fact]
    public void Constructor_WhenGroupIdIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Group> construct = () => CreateSut(
            groupId: null!,
            groupUid: GroupUIDTestDoubles.Create(),
            externalIds: GroupExternalIdentifiersTestDoubles.Create(),
            composition: GroupCompositionTestDoubles.Create());

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenGroupExternalIdentifier_IsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Group> construct = () => CreateSut(
            groupId: GroupIdTestDoubles.Create(),
            groupUid: GroupUIDTestDoubles.Create(),
            externalIds: null!,
            composition: GroupCompositionTestDoubles.Create());

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenGroupsComposition_IsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Group> construct = () => CreateSut(
            groupId: GroupIdTestDoubles.Create(),
            groupUid: GroupUIDTestDoubles.Create(),
            externalIds: GroupExternalIdentifiersTestDoubles.Create(),
            composition: null!);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenValidArguments_SetsProperties()
    {
        // Arrange
        GroupId id = GroupIdTestDoubles.Create();
        GroupUID uid = GroupUIDTestDoubles.Create();
        GroupExternalIdentifiers externalIds = GroupExternalIdentifiersTestDoubles.Create();

        GroupComposition composition =
            GroupCompositionTestDoubles.Create(
                academies: AcademyTestDouble.Create(7),
                members: MemberTestDoubles.Create(9),
                trustees: TrusteeTestDoubles.Create(13));

        // Act
        Group result = CreateValidSut(
            groupId: id,
            groupUid: uid,
            externalIds: externalIds,
            composition: composition);

        // Assert
        Assert.Equal(uid, result.GroupUID);
        Assert.Same(externalIds.Ukprn, result.Ukprn);
        Assert.Same(externalIds.CompaniesHouseId, result.CompaniesHouseId);

        Assert.Same(composition.Academies, result.Academies);
        Assert.Same(composition.Members, result.Members);
        Assert.Same(composition.Trustees, result.Trustees);
    }

    [Fact]
    public void Equality_WhenAllPropertiesEqual_ShouldBeEqual()
    {
        // Arrange
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(1);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(1);

        Group left = CreateValidSut(composition: GroupCompositionTestDoubles.Create(members: members, trustees: trustees));
        Group right = CreateValidSut(composition: GroupCompositionTestDoubles.Create(members: members, trustees: trustees));

        // Act & Assert
        Assert.Equal(left, right);
    }

    [Fact]
    public void Equality_WhenCollectionsAreDifferent_ShouldNotBeEqual()
    {
        // Arrange
        Group left = CreateValidSut(
            composition: GroupCompositionTestDoubles.Create(
                members: MemberTestDoubles.Create(count: 1)));

        Group right = CreateValidSut(
            composition: GroupCompositionTestDoubles.Create(
                members: MemberTestDoubles.Create(count: 1)));

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
    public void Equality_WhenGroupIdIsNull_ShouldNotBeEqual()
    {
        // Arrange
        Group left = CreateValidSut(groupId: GroupIdTestDoubles.Create("group-1"));
        Group? right = null;

        // Act & Assert
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void GetHashCode_WhenEqual_ShouldReturnSameValue()
    {
        // Arrange
        GroupId id = GroupIdTestDoubles.Create();
        GroupUID uid = GroupUIDTestDoubles.Create();
        GroupExternalIdentifiers externalIds = GroupExternalIdentifiersTestDoubles.Create();
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(7);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(9);
        IReadOnlyCollection<Academy> academies = AcademyTestDouble.Create(13);

        GroupComposition composition = GroupCompositionTestDoubles.Create(members: members, trustees: trustees, academies: academies);

        // Act
        Group left = CreateValidSut(
            groupId: id,
            groupUid: uid,
            externalIds: externalIds,
            composition);

        Group right = CreateValidSut(
            groupId: id,
            groupUid: uid,
            externalIds: externalIds,
            composition);

        // Act
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    private static Group CreateSut(
        GroupId groupId,
        GroupUID groupUid,
        GroupExternalIdentifiers externalIds,
        GroupComposition composition)
    {
        return new Group(
                new(groupId, groupUid),
                externalIds,
                composition
            );
    }

    private static Group CreateValidSut(
        GroupId? groupId = null,
        GroupUID? groupUid = null,
        GroupExternalIdentifiers? externalIds = null,
        GroupComposition? composition = null)
    {
        return CreateSut(
            groupId ?? GroupIdTestDoubles.Create(),
            groupUid ?? GroupUIDTestDoubles.Create(),
            externalIds ??
                new(
                    UkprnTestDoubles.Create(),
                    CompaniesHouseIdTestDoubles.Create()),
            composition ?? GroupCompositionTestDoubles.Create()
        );
    }
}
