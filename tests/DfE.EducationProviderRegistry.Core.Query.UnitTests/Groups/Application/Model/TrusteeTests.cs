using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class TrusteeTests
{
    [Fact]
    public void Constructor_GivenNullId_ShouldThrowArgumentNullException()
    {
        // Arrange
        GovernanceIdentifier id = null!;
        Name name = NameTestDoubles.Create();
        DateTime startDate = DateTime.UtcNow;

        Func<Trustee> construct = () => new Trustee(id, name, startDate);

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

        Func<Trustee> construct = () => new Trustee(id, name, startDate);

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
        TrusteeTitle title = TrusteeTestDoubles.CreateTrusteeTitle();

        // Act
        Trustee trustee = new(id, name, startDate, title);

        // Assert
        Assert.Equal(id, trustee.Id);
        Assert.Equal(name, trustee.Name);
        Assert.Equal(startDate, trustee.StartDate);
        Assert.Equal(title, trustee.Title);
    }

    [Fact]
    public void Constructor_GivenNoTitle_ShouldSetTitleToNull()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        DateTime startDate = new(2024, 01, 01);

        // Act
        Trustee trustee = new(id, name, startDate);

        // Assert
        Assert.Null(trustee.Title);
    }

    [Fact]
    public void Equality_GivenSameValues_ShouldBeEqual()
    {
        // Arrange
        GovernanceIdentifier id = GovernanceIdentifierTestDoubles.Create();
        Name name = NameTestDoubles.Create();
        DateTime startDate = new(2024, 01, 01);
        TrusteeTitle title = TrusteeTestDoubles.CreateTrusteeTitle();

        Trustee first = new(id, name, startDate, title);
        Trustee second = new(id, name, startDate, title);

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
            new DateTime(2024, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create("7654321"),
            NameTestDoubles.Create(),
            new DateTime(2024, 01, 01));

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
            new DateTime(2024, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create("Name2"),
            new DateTime(2024, 01, 01));

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
            new DateTime(2024, 01, 01));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            new DateTime(2025, 01, 01));

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
            new DateTime(2024, 01, 01),
            TrusteeTestDoubles.CreateTrusteeTitle(TrusteeTitleType.Chair));

        Trustee second = new(
            GovernanceIdentifierTestDoubles.Create(),
            NameTestDoubles.Create(),
            new DateTime(2024, 01, 01),
            TrusteeTestDoubles.CreateTrusteeTitle(TrusteeTitleType.Other));

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
