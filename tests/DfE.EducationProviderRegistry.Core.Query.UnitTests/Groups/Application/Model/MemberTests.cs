using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Shared;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class MemberTests
{
    [Fact]
    public void Constructor_GivenNullId_ShouldThrowArgumentNullException()
    {
        // Arrange
        GovernanceIdentifier id = null!;
        Name name = NameTestDoubles.Create();
        DateOnly startDate = new(2024, 01, 01);

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
        DateOnly startDate = new(2024, 01, 01);

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
        DateOnly startDate = new(2024, 01, 01);
        DateOnly endDate = new(2028, 01, 01);
        string title = "Mr";
        string appointedBy = "Appointed by academy members";

        // Act
        Member member = new(
            id,
            name,
            startDate,
            endDate,
            title,
            appointedBy);

        // Assert
        Assert.Equal(id, member.Id);
        Assert.Equal(name, member.Name);
        Assert.Equal(startDate, member.StartDate);
        Assert.Equal(endDate, member.EndDate);
        Assert.Equal(title, member.Title);
        Assert.Equal(appointedBy, member.AppointedBy);
    }

    [Fact]
    public void Constructor_GivenOptionalArgumentsNotProvided_ShouldSetPropertiesToNull()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        DateOnly startDate = new(2024, 01, 01);

        // Act
        Member member = new(id, name, startDate);

        // Assert
        Assert.Null(member.EndDate);
        Assert.Null(member.Title);
        Assert.Null(member.AppointedBy);
    }

    [Fact]
    public void Equality_GivenSameValues_ShouldBeEqual()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        DateOnly startDate = new(2024, 01, 01);
        DateOnly endDate = new(2028, 01, 01);
        string title = "Mr";
        string appointedBy = "Appointed by academy members";

        Member first = new(
            id,
            name,
            startDate,
            endDate,
            title,
            appointedBy);

        Member second = new(
            id,
            name,
            startDate,
            endDate,
            title,
            appointedBy);

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
            new DateOnly(2024, 01, 01));

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("7654321"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2024, 01, 01));

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
            new DateOnly(2024, 01, 01));

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name2"),
            new DateOnly(2024, 01, 01));

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
            new DateOnly(2024, 01, 01));

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2025, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentEndDate_ShouldNotBeEqual()
    {
        // Arrange
        Member first = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2024, 01, 01),
            new DateOnly(2028, 01, 01));

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2024, 01, 01),
            new DateOnly(2029, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentTitle_ShouldNotBeEqual()
    {
        // Arrange
        Member first = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2024, 01, 01),
            title: "Mr");

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2024, 01, 01),
            title: "Mrs");

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentAppointedBy_ShouldNotBeEqual()
    {
        // Arrange
        Member first = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2024, 01, 01),
            appointedBy: "Appointed by academy members");

        Member second = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create("Name"),
            new DateOnly(2024, 01, 01),
            appointedBy: "Appointed by GB/board");

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
