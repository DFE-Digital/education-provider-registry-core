using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Infrastructure;

public sealed class GroupMappingContextToGroupMapperTests
{
    [Fact]
    public void Constructor_Should_Throw_When_EstablishmentMapper_Is_Null()
    {
        // Arrange
        Func<GroupMappingContextToGroupMapper> construct = () => new(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_Should_Throw_When_Input_Is_Null()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        Func<Group> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_Should_Map_Identity()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithGroupId("TR001")
            .WithGroupUid(12345)
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal("TR001", result.GroupId!.Value);
        Assert.Equal(12345, result.GroupUID.Value);
    }

    [Fact]
    public void Map_Should_Map_Null_GroupId()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithGroupId(null)
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Null(result.GroupId);
    }

    [Fact]
    public void Map_Should_Map_ExternalIdentifiers()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithUkprn("10000001")
            .WithCompaniesHouseNumber("COMP123")
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

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
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithCompaniesHouseNumber(null)
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Null(result.CompaniesHouseId);
    }

    [Fact]
    public void Map_Should_Map_Null_Ukprn()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithUkprn(null)
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Null(result.Ukprn);
    }

    [Fact]
    public void Map_Should_Map_Composition()
    {
        // Arrange
        IReadOnlyCollection<Establishment> establishments =
            EstablishmentTestDoubles.Create(3);

        Mock<
            IMapper<
                IEnumerable<GroupEstablishmentAggregate>,
                IReadOnlyCollection<Establishment>>> establishmentMapper = CreateEstablishmentMapperMock(establishments);

        GroupMappingContextToGroupMapper sut = new(establishmentMapper.Object);

        GroupAggregate group =
            GroupAggregateBuilder.Create()
                .Build();

        GroupEstablishmentAggregate[] establishmentAggregates =
        [
            new GroupEstablishmentAggregate(),
            new GroupEstablishmentAggregate(),
            new GroupEstablishmentAggregate()
        ];

        GroupMappingContext input = CreateMappingContext(
            group,
            establishmentAggregates);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal(establishments, result.Establishments);
        Assert.Empty(result.Members);
        Assert.Empty(result.Trustees);
    }

    [Fact]
    public void Map_Should_Call_EstablishmentMapper()
    {
        // Arrange
        GroupEstablishmentAggregate[] establishmentAggregates =
        [
            new GroupEstablishmentAggregate(),
            new GroupEstablishmentAggregate()
        ];

        Mock<IMapper<IEnumerable<GroupEstablishmentAggregate>, IReadOnlyCollection<Establishment>>>
            establishmentMapper = CreateEstablishmentMapperMock();

        GroupMappingContextToGroupMapper sut = new(
            establishmentMapper.Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .Build();

        GroupMappingContext input = CreateMappingContext(
            group,
            establishmentAggregates);

        // Act
        sut.Map(input);

        // Assert
        establishmentMapper.VerifyMapperCalledWith(establishmentAggregates);
    }

    [Fact]
    public void Map_Should_Map_GroupType()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithTypeName("Federation")
            .WithTypeCode("01")
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal("Federation", result.GroupType.Name);
        Assert.Equal("01", result.GroupType.Code.Value);
    }

    [Fact]
    public void Map_Should_Map_Null_Status_When_No_Status_Date_Is_Present()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithStatus(
                label: "Closed",
                effectiveDate: null)
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Null(result.Status);
    }

    [Fact]
    public void Map_Should_Map_Status()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithStatus(
                label: "Closed",
                effectiveDate: new DateOnly(2020, 10, 10))
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.NotNull(result.Status);
        Assert.Equal("Closed", result.Status.Label);
        Assert.Equal(
            new DateOnly(2020, 10, 10),
            result.Status.EffectiveDate);
    }

    [Fact]
    public void Map_Should_Map_Null_Address_Values_As_Empty_Strings()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithSiteName(null)
            .WithAddressLine1(null)
            .WithAddressLine2(null)
            .WithTown(null)
            .WithCounty(null)
            .WithPostcode(null)
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

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
        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithSiteName("Head Office")
            .WithAddressLine1("Line 1")
            .WithAddressLine2("Line 2")
            .WithTown("Town")
            .WithCounty("County")
            .WithPostcode("AB1 2CD")
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

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

    private static GroupMappingContext CreateMappingContext(
        GroupAggregate group,
        IReadOnlyCollection<GroupEstablishmentAggregate>? establishments = null)
    {
        return new GroupMappingContext(
            group,
            establishments ?? []);
    }

    private static Mock<
        IMapper<
            IEnumerable<GroupEstablishmentAggregate>,
            IReadOnlyCollection<Establishment>>>
        CreateEstablishmentMapperMock(
            IReadOnlyCollection<Establishment>? mapOut = null)
    {
        return IMapperTestDouble.Map<
            IEnumerable<GroupEstablishmentAggregate>,
            IReadOnlyCollection<Establishment>>(
                mapOut ?? []);
    }
}
