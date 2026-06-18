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
            companiesHouseId: CompaniesHouseIdTestDoubles.Create(),
            academies: [],
            trustees: [],
            members: []);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenCompaniesHouseIdIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Group> construct = () => CreateSut(
            groupId: GroupIdTestDoubles.Create(),
            groupUid: GroupUIDTestDoubles.Create(),
            companiesHouseId: null!,
            academies: [],
            trustees: [],
            members: []);

        // Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenValidArguments_SetsProperties()
    {
        // Arrange
        GroupId id = GroupIdTestDoubles.Create();
        GroupUID uid = GroupUIDTestDoubles.Create();
        CompaniesHouseId companiesHouseId = CompaniesHouseIdTestDoubles.Create();
        IReadOnlyCollection<Academy> academies = AcademyTestDouble.Create(7);
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(9);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(13);

        // Act
        Group result = CreateValidSut(
            groupId: id,
            groupUid: uid,
            companiesHouseId: companiesHouseId,
            academies: academies,
            members: members,
            trustees: trustees);

        // Assert
        Assert.Equal(uid, result.GroupUID);
        Assert.Equal(companiesHouseId, result.CompaniesHouseId);

        Assert.Equivalent(academies, result.Academies);
        Assert.Equivalent(members, result.Members);
        Assert.Equivalent(trustees, result.Trustees);
    }

    [Fact]
    public void Constructor_WhenOptionalCollectionsAreNull_SetsEmptyCollections()
    {
        // Arrange

        // Act
        Group result = CreateValidSut(
            academies: null,
            members: null,
            trustees: null);

        // Assert
        Assert.Empty(result.Academies);
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
    public void Equality_WhenCollectionsAreDifferent_ShouldNotBeEqual()
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
        CompaniesHouseId companiesHouseId = CompaniesHouseIdTestDoubles.Create();
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(7);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(9);
        IReadOnlyCollection<Academy> academies = AcademyTestDouble.Create(13);

        // Act
        Group left = CreateValidSut(
            groupId: id,
            groupUid: uid,
            companiesHouseId: companiesHouseId,
            academies: academies,
            members: members,
            trustees: trustees);

        Group right = CreateValidSut(
            groupId: id,
            groupUid: uid,
            companiesHouseId: companiesHouseId,
            academies: academies,
            members: members,
            trustees: trustees);

        // Act
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    private static Group CreateSut(
        GroupId groupId,
        GroupUID groupUid,
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
        GroupUID? groupUid = null,
        CompaniesHouseId? companiesHouseId = null,
        IReadOnlyCollection<Academy>? academies = null,
        IReadOnlyCollection<Member>? members = null,
        IReadOnlyCollection<Trustee>? trustees = null)
    {
        return CreateSut(
            groupId ?? GroupIdTestDoubles.Create(),
            groupUid ?? GroupUIDTestDoubles.Create(),
            companiesHouseId ?? CompaniesHouseIdTestDoubles.Create(),
            academies ?? [],
            members ?? [],
            trustees ?? []);
    }
}
