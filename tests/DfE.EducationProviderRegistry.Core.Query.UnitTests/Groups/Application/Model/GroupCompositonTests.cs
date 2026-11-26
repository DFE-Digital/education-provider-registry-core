using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupCompositionTests
{
    [Fact]
    public void Constructor_WhenOptionalCollectionsAreNull_SetsEmptyCollections()
    {
        // Arrange

        // Act
        GroupComposition result = new(
            members: null,
            trustees: null,
            establishments: null);

        // Assert
        Assert.Empty(result.Members);
        Assert.Empty(result.Trustees);
        Assert.Empty(result.Establishments);
    }

    [Fact]
    public void Constructor_WhenCollectionsProvided_AssignsValues()
    {
        // Arrange
        IReadOnlyCollection<Establishment> establishments =
            EstablishmentTestDoubles.Create(2);

        IReadOnlyCollection<Member> members =
            MemberTestDoubles.Create(2);

        IReadOnlyCollection<Trustee> trustees =
            TrusteeTestDoubles.Create(2);

        // Act
        GroupComposition result = new(
            members,
            trustees,
            establishments);

        // Assert
        Assert.Equal(members, result.Members);
        Assert.Equal(trustees, result.Trustees);
        Assert.Equal(establishments, result.Establishments);
    }

    [Fact]
    public void Constructor_WhenCollectionsProvided_CreatesCopies()
    {
        // Arrange
        IReadOnlyCollection<Establishment> establishments =
            EstablishmentTestDoubles.Create(2);

        IReadOnlyCollection<Member> members =
            MemberTestDoubles.Create(2);

        IReadOnlyCollection<Trustee> trustees =
            TrusteeTestDoubles.Create(2);

        // Act
        GroupComposition result = new(
            members,
            trustees,
            establishments);

        // Assert
        Assert.NotSame(members, result.Members);
        Assert.NotSame(trustees, result.Trustees);
        Assert.NotSame(establishments, result.Establishments);
    }

    [Fact]
    public void Equals_WhenOtherIsNull_ReturnsFalse()
    {
        // Arrange
        GroupComposition composition = GroupCompositionTestDoubles.Create(
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.Create(),
            EstablishmentTestDoubles.Create());

        // Act
        bool result = composition.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenAllCollectionsMatch_ReturnsTrue()
    {
        // Arrange
        IReadOnlyCollection<Establishment> establishments =
            EstablishmentTestDoubles.Create(1);

        IReadOnlyCollection<Member> members =
            MemberTestDoubles.CreateWith(
                ("1234567", "Member A", new DateOnly(2020, 01, 01)));

        IReadOnlyCollection<Trustee> trustees =
            TrusteeTestDoubles.CreateWith(
                ("7654321", "Trustee A", "Trustee", new DateOnly(2021, 01, 01)));

        GroupComposition left = GroupCompositionTestDoubles.Create(
            members,
            trustees,
            establishments);

        GroupComposition right = GroupCompositionTestDoubles.Create(
            members,
            trustees,
            establishments);

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WhenEstablishmentsDiffer_ReturnsFalse()
    {
        // Arrange
        IReadOnlyCollection<Establishment> leftEstablishments =
            EstablishmentTestDoubles.Create(1);

        IReadOnlyCollection<Establishment> rightEstablishments =
            EstablishmentTestDoubles.Create(2);

        GroupComposition left = new(
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.Create(),
            leftEstablishments);

        GroupComposition right = new(
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.Create(),
            rightEstablishments);

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenMembersDiffer_ReturnsFalse()
    {
        // Arrange
        GroupComposition left = new(
            MemberTestDoubles.CreateWith(
                (
                    Id: "1111111",
                    Name: "Member A",
                    StartDate: new DateOnly(2020, 01, 01))),
            TrusteeTestDoubles.Create(),
            EstablishmentTestDoubles.Create());

        GroupComposition right = new(
            MemberTestDoubles.CreateWith(
                (
                    Id: "2222222",
                    Name: "Member B",
                    StartDate: new DateOnly(2020, 01, 01))),
            TrusteeTestDoubles.Create(),
            EstablishmentTestDoubles.Create());

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenTrusteesDiffer_ReturnsFalse()
    {
        // Arrange
        GroupComposition left = new(
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.CreateWith(
                (
                    Id: "1111111",
                    Name: "Trustee A",
                    Role: "Trustee",
                    StartDate: new DateOnly(2020, 01, 01))),
            EstablishmentTestDoubles.Create());

        GroupComposition right = new(
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.CreateWith(
                (
                    Id: "2222222",
                    Name: "Trustee B",
                    Role: "Trustee",
                    StartDate: new DateOnly(2020, 01, 01))),
            EstablishmentTestDoubles.Create());

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }
}
