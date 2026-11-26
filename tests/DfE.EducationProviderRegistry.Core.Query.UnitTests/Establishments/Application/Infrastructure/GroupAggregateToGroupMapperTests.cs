using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Shared;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Infrastructure;

public sealed class GroupMappingContextToGroupMapperTests
{
    [Fact]
    public void Constructor_Should_Throw_When_EstablishmentMapper_Is_Null()
    {
        // Arrange
        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Member>>> memberMapper =
                    CreateMemberMapperMock();

        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Trustee>>> trusteeMapper =
                    CreateTrusteeMapperMock();

        Func<GroupMappingContextToGroupMapper> construct = () =>
            new(
                null!,
                memberMapper.Object,
                trusteeMapper.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_MemberMapper_Is_Null()
    {
        // Arrange
        Mock<
            IMapper<
                IEnumerable<GroupEstablishmentAggregate>,
                IReadOnlyCollection<Establishment>>> establishmentMapper =
                    CreateEstablishmentMapperMock();

        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Trustee>>> trusteeMapper =
                    CreateTrusteeMapperMock();

        Func<GroupMappingContextToGroupMapper> construct = () =>
            new(
                establishmentMapper.Object,
                null!,
                trusteeMapper.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_TrusteeMapper_Is_Null()
    {
        // Arrange
        Mock<
            IMapper<
                IEnumerable<GroupEstablishmentAggregate>,
                IReadOnlyCollection<Establishment>>> establishmentMapper =
                    CreateEstablishmentMapperMock();

        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Member>>> memberMapper =
                    CreateMemberMapperMock();

        Func<GroupMappingContextToGroupMapper> construct = () =>
            new(
                establishmentMapper.Object,
                memberMapper.Object,
                null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_Should_Throw_When_Input_Is_Null()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = CreateSut();

        Func<Group> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_Should_Map_Identity()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = CreateSut();

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
        GroupMappingContextToGroupMapper sut = CreateSut();

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
        GroupMappingContextToGroupMapper sut = CreateSut();

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
        GroupMappingContextToGroupMapper sut = CreateSut();

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
        GroupMappingContextToGroupMapper sut = CreateSut();

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

        IReadOnlyCollection<Member> members =
        [
            new(
                GovernanceIdentifierTestDoubles.Create("1000205"),
                NameTestDoubles.Create("David Evans"),
                new DateOnly(2016, 12, 05),
                new DateOnly(2028, 08, 27),
                "Mr",
                "Appointed by academy members")
        ];

        IReadOnlyCollection<Trustee> trustees =
        [
            new(
                GovernanceIdentifierTestDoubles.Create("1548386"),
                NameTestDoubles.Create("Simon Sanger-Anderson"),
                "Chair of Trustees",
                new DateOnly(2016, 12, 05),
                new DateOnly(2026, 08, 27),
                "Mr",
                "Appointed by trustees")
        ];

        Mock<
            IMapper<
                IEnumerable<GroupEstablishmentAggregate>,
                IReadOnlyCollection<Establishment>>> establishmentMapper =
                    CreateEstablishmentMapperMock(establishments);

        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Member>>> memberMapper =
                    CreateMemberMapperMock(members);

        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Trustee>>> trusteeMapper =
                    CreateTrusteeMapperMock(trustees);

        GroupMappingContextToGroupMapper sut = new(
            establishmentMapper.Object,
            memberMapper.Object,
            trusteeMapper.Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .Build();

        GroupEstablishmentAggregate[] establishmentAggregates =
        [
            new(),
            new(),
            new()
        ];

        GroupGovernorAggregate[] governorAggregates =
        [
            new(),
            new()
        ];

        GroupMappingContext input = CreateMappingContext(
            group,
            establishmentAggregates,
            governorAggregates);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.Equal(establishments, result.Establishments);
        Assert.Equal(members, result.Members);
        Assert.Equal(trustees, result.Trustees);
    }

    [Fact]
    public void Map_Should_Call_EstablishmentMapper()
    {
        // Arrange
        GroupEstablishmentAggregate[] establishmentAggregates =
        [
            new(),
            new()
        ];

        Mock<
            IMapper<
                IEnumerable<GroupEstablishmentAggregate>,
                IReadOnlyCollection<Establishment>>> establishmentMapper =
                    CreateEstablishmentMapperMock();

        GroupMappingContextToGroupMapper sut = new(
            establishmentMapper.Object,
            CreateMemberMapperMock().Object,
            CreateTrusteeMapperMock().Object);

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
    public void Map_Should_Call_MemberMapper()
    {
        // Arrange
        GroupGovernorAggregate[] governorAggregates =
        [
            new(),
            new()
        ];

        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Member>>> memberMapper =
                    CreateMemberMapperMock();

        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object,
            memberMapper.Object,
            CreateTrusteeMapperMock().Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .Build();

        GroupMappingContext input = CreateMappingContext(
            group,
            governors: governorAggregates);

        // Act
        sut.Map(input);

        // Assert
        memberMapper.VerifyMapperCalledWith(governorAggregates);
    }

    [Fact]
    public void Map_Should_Call_TrusteeMapper()
    {
        // Arrange
        GroupGovernorAggregate[] governorAggregates =
        [
            new(),
            new()
        ];

        Mock<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Trustee>>> trusteeMapper =
                    CreateTrusteeMapperMock();

        GroupMappingContextToGroupMapper sut = new(
            CreateEstablishmentMapperMock().Object,
            CreateMemberMapperMock().Object,
            trusteeMapper.Object);

        GroupAggregate group = GroupAggregateBuilder.Create()
            .Build();

        GroupMappingContext input = CreateMappingContext(
            group,
            governors: governorAggregates);

        // Act
        sut.Map(input);

        // Assert
        trusteeMapper.VerifyMapperCalledWith(governorAggregates);
    }

    [Fact]
    public void Map_Should_Map_GroupType()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = CreateSut();

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
    public void Map_Should_Map_Null_Status_When_No_Status_Label_Is_Present()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = CreateSut();

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithStatus(
                label: null,
                effectiveDate: null,
                openedDate: null,
                closedDate: null)
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
        GroupMappingContextToGroupMapper sut = CreateSut();

        GroupAggregate group = GroupAggregateBuilder.Create()
            .WithStatus(
                label: "Open",
                effectiveDate: null,
                closedDate: null,
                openedDate: new DateOnly(2020, 10, 10))
            .Build();

        GroupMappingContext input = CreateMappingContext(group);

        // Act
        Group result = sut.Map(input);

        // Assert
        Assert.NotNull(result.Status);
        Assert.Equal("Open", result.Status.Label);
        Assert.Equal(
            new DateOnly(2020, 10, 10),
            result.Status.OpenedDate);
    }

    [Fact]
    public void Map_Should_Map_Null_Address_Values_As_Empty_Strings()
    {
        // Arrange
        GroupMappingContextToGroupMapper sut = CreateSut();

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
        GroupMappingContextToGroupMapper sut = CreateSut();

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

    private static GroupMappingContextToGroupMapper CreateSut()
    {
        return new(
            CreateEstablishmentMapperMock().Object,
            CreateMemberMapperMock().Object,
            CreateTrusteeMapperMock().Object);
    }

    private static GroupMappingContext CreateMappingContext(
        GroupAggregate group,
        IReadOnlyCollection<GroupEstablishmentAggregate>? establishments = null,
        IReadOnlyCollection<GroupGovernorAggregate>? governors = null)
    {
        return new(
            group,
            establishments ?? [],
            governors ?? []);
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

    private static Mock<
        IMapper<
            IEnumerable<GroupGovernorAggregate>,
            IReadOnlyCollection<Member>>>
        CreateMemberMapperMock(
            IReadOnlyCollection<Member>? mapOut = null)
    {
        return IMapperTestDouble.Map<
            IEnumerable<GroupGovernorAggregate>,
            IReadOnlyCollection<Member>>(
                mapOut ?? []);
    }

    private static Mock<
        IMapper<
            IEnumerable<GroupGovernorAggregate>,
            IReadOnlyCollection<Trustee>>>
        CreateTrusteeMapperMock(
            IReadOnlyCollection<Trustee>? mapOut = null)
    {
        return IMapperTestDouble.Map<
            IEnumerable<GroupGovernorAggregate>,
            IReadOnlyCollection<Trustee>>(
                mapOut ?? []);
    }
}
