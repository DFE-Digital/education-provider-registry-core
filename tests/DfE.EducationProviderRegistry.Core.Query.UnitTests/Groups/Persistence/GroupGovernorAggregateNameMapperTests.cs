using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Persistence;

public sealed class GroupGovernorAggregateNameMapperTests
{
    [Fact]
    public void CreateName_GivenFirstNameAndLastName_ShouldCreateFullName()
    {
        // Arrange
        GroupGovernorAggregate governor = new()
        {
            FirstName = "Simon",
            LastName = "Sanger-Anderson"
        };

        // Act
        Name result = GroupGovernorAggregateNameMapper.CreateName(governor);

        // Assert
        Assert.Equal("Simon Sanger-Anderson", result.Value);
    }

    [Fact]
    public void CreateName_GivenNullFirstName_ShouldCreateNameFromLastName()
    {
        // Arrange
        GroupGovernorAggregate governor = new()
        {
            FirstName = null,
            LastName = "Sanger-Anderson"
        };

        // Act
        Name result = GroupGovernorAggregateNameMapper.CreateName(governor);

        // Assert
        Assert.Equal("Sanger-Anderson", result.Value);
    }

    [Fact]
    public void CreateName_GivenEmptyFirstName_ShouldCreateNameFromLastName()
    {
        // Arrange
        GroupGovernorAggregate governor = new()
        {
            FirstName = string.Empty,
            LastName = "Sanger-Anderson"
        };

        // Act
        Name result = GroupGovernorAggregateNameMapper.CreateName(governor);

        // Assert
        Assert.Equal("Sanger-Anderson", result.Value);
    }

    [Fact]
    public void CreateName_GivenWhitespaceFirstName_ShouldCreateNameFromLastName()
    {
        // Arrange
        GroupGovernorAggregate governor = new()
        {
            FirstName = " ",
            LastName = "Sanger-Anderson"
        };

        // Act
        Name result = GroupGovernorAggregateNameMapper.CreateName(governor);

        // Assert
        Assert.Equal("Sanger-Anderson", result.Value);
    }
}
