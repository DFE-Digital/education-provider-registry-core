using System.ComponentModel.DataAnnotations;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupExternalIdentifiersTests
{
    [Fact]
    public void Constructor_Should_Assign_Values_When_All_Provided()
    {
        // Arrange
        Ukprn ukprn = UkprnTestDoubles.Create("12345678");
        CompaniesHouseId companiesHouseId = CompaniesHouseIdTestDoubles.Create("09876543");

        // Act
        GroupExternalIdentifiers result = new(ukprn, companiesHouseId);

        // Assert
        Assert.Equal(ukprn, result.Ukprn);
        Assert.Equal(companiesHouseId, result.CompaniesHouseId);
    }

    [Fact]
    public void Constructor_Should_Allow_Null_Ukprn()
    {
        // Arrange
        Ukprn? ukprn = null;
        CompaniesHouseId companiesHouseId = CompaniesHouseIdTestDoubles.Create("09876543");

        // Act
        GroupExternalIdentifiers result = new(ukprn, companiesHouseId);

        // Assert
        Assert.Null(result.Ukprn);
        Assert.Equal(companiesHouseId, result.CompaniesHouseId);
    }

    [Fact]
    public void Constructor_Should_Allow_Null_CompaniesHouseId()
    {
        // Arrange
        Ukprn ukprn = UkprnTestDoubles.Create("12345678");
        CompaniesHouseId? companiesHouseId = null;

        // Act
        GroupExternalIdentifiers result = new(ukprn, companiesHouseId);

        // Assert
        Assert.Equal(ukprn, result.Ukprn);
        Assert.Null(result.CompaniesHouseId);
    }

    [Fact]
    public void Constructor_Should_Allow_Both_Values_To_Be_Null()
    {
        // Arrange
        Ukprn? ukprn = null;
        CompaniesHouseId? companiesHouseId = null;

        // Act
        GroupExternalIdentifiers result = new(ukprn, companiesHouseId);

        // Assert
        Assert.Null(result.Ukprn);
        Assert.Null(result.CompaniesHouseId);
    }

    [Fact]
    public void Equals_Should_Return_True_When_Values_Are_The_Same()
    {
        // Arrange
        GroupExternalIdentifiers left = new(
            UkprnTestDoubles.Create("12345678"),
            CompaniesHouseIdTestDoubles.Create("09876543"));

        GroupExternalIdentifiers right = new(
            UkprnTestDoubles.Create("12345678"),
            CompaniesHouseIdTestDoubles.Create("09876543"));

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_Should_Return_False_When_Ukprn_Differ()
    {
        // Arrange
        GroupExternalIdentifiers left = new(
            UkprnTestDoubles.Create("123"),
            CompaniesHouseIdTestDoubles.Create("1"));

        GroupExternalIdentifiers right = new(
            UkprnTestDoubles.Create("987"),
            CompaniesHouseIdTestDoubles.Create("1"));

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_Should_Return_False_When_Compared_Ukprn_IsNull()
    {
        // Arrange
        GroupExternalIdentifiers left = new(
            UkprnTestDoubles.Create("123"),
            CompaniesHouseIdTestDoubles.Create("1"));

        GroupExternalIdentifiers right = new(
            ukprn: null!,
            CompaniesHouseIdTestDoubles.Create("1"));

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_Should_Return_False_When_CompaniesHouseid_Differ()
    {
        // Arrange
        GroupExternalIdentifiers left = new(
            UkprnTestDoubles.Create("1"),
            CompaniesHouseIdTestDoubles.Create("12345678"));

        GroupExternalIdentifiers right = new(
            UkprnTestDoubles.Create("1"),
            CompaniesHouseIdTestDoubles.Create("09876543"));

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_Should_Return_False_When_Compared_CompaniesHouseId_IsNull()
    {
        // Arrange
        GroupExternalIdentifiers left = new(
            UkprnTestDoubles.Create("123"),
            CompaniesHouseIdTestDoubles.Create("1"));

        GroupExternalIdentifiers right = new(
            UkprnTestDoubles.Create("123"),
            companiesHouseId: null);

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }
}
