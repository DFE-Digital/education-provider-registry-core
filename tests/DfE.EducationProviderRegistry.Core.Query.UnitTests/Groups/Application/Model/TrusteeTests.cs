using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Shared;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class TrusteeTests
{
    [Fact]
    public void Constructor_GivenNullId_ShouldThrowArgumentNullException()
    {
        // Arrange
        GovernanceIdentifier id = null!;
        Name name = NameTestDoubles.Create();
        string role = "Trustee";
        DateOnly startDate = new(2024, 01, 01);

        Func<Trustee> construct = () =>
            new Trustee(id, name, role, startDate);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_GivenNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = null!;
        string role = "Trustee";
        DateOnly startDate = new(2024, 01, 01);

        Func<Trustee> construct = () =>
            new Trustee(id, name, role, startDate);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenInvalidRole_ShouldThrowArgumentException(string? role)
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        DateOnly startDate = new(2024, 01, 01);

        Func<Trustee> construct = () =>
            new Trustee(id, name, role!, startDate);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_GivenValidArguments_ShouldSetProperties()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        string role = "Chief Financial Officer";
        string title = "Mrs";
        string appointedBy = "Appointed by academy members";
        DateOnly startDate = new(2024, 01, 01);
        DateOnly endDate = new(2028, 01, 01);

        // Act
        Trustee trustee = new(
            id,
            name,
            role,
            startDate,
            endDate,
            title,
            appointedBy);

        // Assert
        Assert.Equal(id, trustee.Id);
        Assert.Equal(name, trustee.Name);
        Assert.Equal(role, trustee.Role);
        Assert.Equal(title, trustee.Title);
        Assert.Equal(appointedBy, trustee.AppointedBy);
        Assert.Equal(startDate, trustee.StartDate);
        Assert.Equal(endDate, trustee.EndDate);
    }

    [Fact]
    public void Constructor_GivenOptionalArgumentsNotProvided_ShouldSetPropertiesToNull()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        string role = "Trustee";
        DateOnly startDate = new(2024, 01, 01);

        // Act
        Trustee trustee = new(id, name, role, startDate);

        // Assert
        Assert.Null(trustee.Title);
        Assert.Null(trustee.AppointedBy);
        Assert.Null(trustee.EndDate);
    }

    [Fact]
    public void Equality_GivenSameValues_ShouldBeEqual()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        string role = "Trustee";
        string title = "Mr";
        string appointedBy = "Appointed by academy members";
        DateOnly startDate = new(2024, 01, 01);
        DateOnly endDate = new(2028, 01, 01);

        Trustee first = new(
            id,
            name,
            role,
            startDate,
            endDate,
            title,
            appointedBy);

        Trustee second = new(
            id,
            name,
            role,
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
        Trustee first = new(
            GovernanceIdentifierTestDoubles.Create("1234567"),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create("7654321"),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentName_ShouldNotBeEqual()
    {
        // Arrange
        Trustee first = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create("Name1"),
            "Trustee",
            new DateOnly(2024, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create("Name2"),
            "Trustee",
            new DateOnly(2024, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentRole_ShouldNotBeEqual()
    {
        // Arrange
        Trustee first = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Accounting Officer",
            new DateOnly(2024, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentStartDate_ShouldNotBeEqual()
    {
        // Arrange
        Trustee first = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2025, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentEndDate_ShouldNotBeEqual()
    {
        // Arrange
        Trustee first = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01),
            new DateOnly(2028, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01),
            new DateOnly(2029, 01, 01));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentTitle_ShouldNotBeEqual()
    {
        // Arrange
        Trustee first = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01),
            title: "Mr");

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01),
            title: "Mrs");

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentAppointedBy_ShouldNotBeEqual()
    {
        // Arrange
        Trustee first = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01),
            appointedBy: "Appointed by academy members");

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            "Trustee",
            new DateOnly(2024, 01, 01),
            appointedBy: "Appointed by GB/board");

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
