using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class MemberTests
{
    [Fact]
    public void Constructor_GivenNullId_ShouldThrowArgumentNullException()
    {
        // Arrange
        GovernanceIdentifier id = null!;
        Name name = NameTestDoubles.Create();
        DateTime startDate = DateTime.UtcNow;

        Func<Member> construct =
            () => new Member(id, name, startDate);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_GivenNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = null!;
        DateTime startDate = DateTime.UtcNow;

        Func<Member> construct =
            () => new Member(id, name, startDate);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_GivenValidArguments_ShouldSetProperties()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        DateTime startDate = new(2024, 01, 01);

        // Act
        Member member = new(id, name, startDate);

        // Assert
        Assert.Equal(id, member.Id);
        Assert.Equal(name, member.Name);
        Assert.Equal(startDate, member.StartDate);
    }

    [Fact]
    public void Equality_GivenSameValues_ShouldBeEqual()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        DateTime startDate = new(2024, 01, 01);

        Member first = new(id, name, startDate);
        Member second = new(id, name, startDate);

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentId_ShouldNotBeEqual()
    {
        // Arrange
        Member first = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateTime(2024, 01, 01));

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("7654321"),
            NameTestDoubles.Create("Name"),
            new DateTime(2024, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentName_ShouldNotBeEqual()
    {
        // Arrange
        Member first = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name1"),
            new DateTime(2024, 01, 01));

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name2"),
            new DateTime(2024, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentStartDate_ShouldNotBeEqual()
    {
        // Arrange
        Member first = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateTime(2024, 01, 01));

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateTime(2025, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

}
