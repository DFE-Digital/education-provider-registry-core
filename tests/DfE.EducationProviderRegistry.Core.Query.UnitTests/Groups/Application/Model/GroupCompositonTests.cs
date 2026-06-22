using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupCompositionTests
{
    [Fact]
    public void Constructor_WhenOptionalCollectionsAreNull_SetsEmptyCollections()
    {
        // Arrange

        // Act
        GroupComposition result = new(academies: null, members: null, trustees: null);

        // Assert
        Assert.Empty(result.Academies);
        Assert.Empty(result.Members);
        Assert.Empty(result.Trustees);
    }

    [Fact]
    public void Constructor_WhenCollectionsProvided_AssignsValues()
    {
        // Arrange
        IReadOnlyCollection<Academy> academies = AcademyTestDouble.Create(2);
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(2);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(2);

        // Act
        GroupComposition result = new(academies, members, trustees);

        // Assert
        Assert.Equal(academies, result.Academies);
        Assert.Equal(members, result.Members);
        Assert.Equal(trustees, result.Trustees);
    }

    [Fact]
    public void Constructor_WhenCollectionsProvided_CreatesCopies()
    {
        // Arrange
        IReadOnlyCollection<Academy> academies = AcademyTestDouble.Create(2);
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(2);
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(2);

        // Act
        GroupComposition result = new(academies, members, trustees);

        // Assert
        Assert.NotSame(academies, result.Academies);
        Assert.NotSame(members, result.Members);
        Assert.NotSame(trustees, result.Trustees);
    }

    [Fact]
    public void Equals_WhenOtherIsNull_ReturnsFalse()
    {
        // Arrange
        GroupComposition composition = GroupCompositionTestDoubles.Create(
            AcademyTestDouble.Create(),
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.Create());

        // Act
        bool result = composition.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenAllCollectionsMatch_ReturnsTrue()
    {
        // Arrange
        IReadOnlyCollection<Academy> academies = AcademyTestDouble.CreateWith(("100001", "Academy A"));
        IReadOnlyCollection<Member> members = MemberTestDoubles.CreateWith(("1234567", "Member A", new DateTime(2020, 01, 01)));
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.CreateWith(("7654321", "Trustee A", new DateTime(2021, 01, 01)));

        GroupComposition left = GroupCompositionTestDoubles.Create(academies, members, trustees);
        GroupComposition right = GroupCompositionTestDoubles.Create(academies, members, trustees);

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WhenAcademiesDiffer_ReturnsFalse()
    {
        // Arrange
        GroupComposition left = new(
            AcademyTestDouble.CreateWith((Urn: "100001", Name: "Academy A")),
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.Create());

        GroupComposition right = new(
            AcademyTestDouble.CreateWith((Urn: "100001", Name: "Academy B")),
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.Create());

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
            AcademyTestDouble.Create(),
            MemberTestDoubles.CreateWith((Id: "1111111", Name: "Member A", StartDate: new DateTime(2020, 01, 01))),
            TrusteeTestDoubles.Create());

        GroupComposition right = new(
            AcademyTestDouble.Create(),
            MemberTestDoubles.CreateWith((Id: "2222222", Name: "Member B", StartDate: new DateTime(2020, 01, 01))),
            TrusteeTestDoubles.Create());

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
            AcademyTestDouble.Create(),
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.CreateWith((Id: "1111111", Name: "Trustee A", StartDate: new DateTime(2020, 01, 01))));

        GroupComposition right = new(
            AcademyTestDouble.Create(),
            MemberTestDoubles.Create(),
            TrusteeTestDoubles.CreateWith((Id: "2222222", Name: "Trustee B", StartDate: new DateTime(2020, 01, 01))));

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }
}
