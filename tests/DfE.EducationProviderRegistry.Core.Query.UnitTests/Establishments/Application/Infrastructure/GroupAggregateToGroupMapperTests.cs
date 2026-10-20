using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Infrastructure;

public sealed class GroupAggregateToGroupMapperTests
{
    [Fact]
    public void Map_Should_Throw_When_Input_Is_Null()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        Func<Group> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_Should_Map_Identity()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .WithGroupId("TR001")
            .WithGroupUid(12345)
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal("TR001", result.GroupId.Value);
        Assert.Equal(12345, result.GroupUID.Value);
    }

    [Fact]
    public void Map_Should_Map_ExternalIdentifiers()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .WithUkprn("10000001")
            .WithCompaniesHouseNumber("COMP123")
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal("10000001", result.Ukprn!.Value);
        Assert.Equal("COMP123", result.CompaniesHouseId!.Value);
    }

    [Fact]
    public void Map_Should_Map_Null_CompaniesHouseId()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .WithCompaniesHouseNumber(null)
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Null(result.CompaniesHouseId);
    }

    [Fact]
    public void Map_Should_Create_Empty_Composition_Collections()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Empty(result.Academies);
        Assert.Empty(result.Members);
        Assert.Empty(result.Trustees);
    }

    [Fact]
    public void Map_Should_Map_Null_Status_When_No_Status_Date_Is_Present()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .WithStatus(
                label: "Closed",
                effectiveDate: null)
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Null(result.Status);
    }

    [Fact]
    public void Map_Should_Map_Status()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .WithStatus("Closed", new DateOnly(2020, 10, 10))
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.NotNull(result.Status);
        Assert.Equal("Closed", result.Status!.Label);
        Assert.Equal(
            new DateOnly(2020, 10, 10),
            result.Status.EffectiveDate);
    }

    [Fact]
    public void Map_Should_Map_Null_Address_Values_As_Empty_Strings()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .WithSiteName(null)
            .WithAddressLine1(null)
            .WithAddressLine2(null)
            .WithTown(null)
            .WithCounty(null)
            .WithPostcode(null)
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal(string.Empty, result.Address.Name);
        Assert.Equal(string.Empty, result.Address.AddressLine1);
        Assert.Equal(string.Empty, result.Address.AddressLine2);
        Assert.Equal(string.Empty, result.Address.Town);
        Assert.Equal(string.Empty, result.Address.County);
        Assert.Equal(string.Empty, result.Address.Postcode);
    }

    [Fact]
    public void Map_Should_Map_Address()
    {
        // Arrange
        GroupAggregateToGroupMapper sut = new();

        GroupAggregate input = GroupAggregateBuilder.Create()
            .WithSiteName("Head Office")
            .WithAddressLine1("Line 1")
            .WithAddressLine2("Line 2")
            .WithTown("Town")
            .WithCounty("County")
            .WithPostcode("AB1 2CD")
            .Build();

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal("Head Office", result.Address.Name);
        Assert.Equal("Line 1", result.Address.AddressLine1);
        Assert.Equal("Line 2", result.Address.AddressLine2);
        Assert.Equal("Town", result.Address.Town);
        Assert.Equal("County", result.Address.County);
        Assert.Equal("AB1 2CD", result.Address.Postcode);
    }
}
