using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Shared;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class GroupToGroupReadModelMapperTests
{
    [Fact]
    public void Constructor_Should_Throw_When_MemberMapper_Is_Null()
    {
        // Arrange
        Func<GroupToGroupReadModelMapper> construct = () => new(
            null!,
            CreateTrusteeMapperMock().Object,
            CreateEstablishmentMapperMock().Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_TrusteeMapper_Is_Null()
    {
        // Arrange
        Func<GroupToGroupReadModelMapper> construct = () => new(
            CreateMemberMapperMock().Object,
            null!,
            CreateEstablishmentMapperMock().Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_EstablishmentMapper_Is_Null()
    {
        // Arrange
        Func<GroupToGroupReadModelMapper> construct = () => new(
            CreateMemberMapperMock().Object,
            CreateTrusteeMapperMock().Object,
            null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_Should_Throw_When_Input_Is_Null()
    {
        // Arrange
        GroupToGroupReadModelMapper sut = new(
            CreateMemberMapperMock().Object,
            CreateTrusteeMapperMock().Object,
            CreateEstablishmentMapperMock().Object);

        Func<GroupReadModel> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_Should_Map_All_Properties()
    {
        // Arrange
        MemberReadModel[] memberReadModels = [];
        TrusteeReadModel[] trusteeReadModels = [];
        EstablishmentReadModel[] establishmentReadModels = [];

        Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>> memberMapper =
            CreateMemberMapperMock(memberReadModels);

        Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>> trusteeMapper =
            CreateTrusteeMapperMock(trusteeReadModels);

        Mock<IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>>> establishmentMapper =
            CreateEstablishmentMapperMock(establishmentReadModels);

        GroupToGroupReadModelMapper sut = new(
            memberMapper.Object,
            trusteeMapper.Object,
            establishmentMapper.Object);

        SiteAddressModel stubAddress = AddressTestDoubles.Generate();

        Group input = new GroupBuilder()
            .WithName("Test Group Name")
            .WithGroupId("Test group")
            .WithGroupUid(123)
            .WithCompaniesHouseId("Corpo")
            .WithUkprn("test-ukprn")
            .WithAddress(
                stubAddress.AddressLine1,
                stubAddress.AddressLine2,
                stubAddress.Town,
                stubAddress.County,
                stubAddress.Postcode)
            .WithGroupStatus("Closed", new(2020, 10, 10))
            .WithType(name: "mat", code: "02")
            .WithEstablishments(EstablishmentTestDoubles.Create(3))
            .WithMembers(MemberTestDoubles.Create(5))
            .WithTrustees(TrusteeTestDoubles.Create(7))
            .Build();

        // Act
        GroupReadModel result = sut.Map(input);

        // Assert
        Assert.Equal("Test Group Name", result.Name);
        Assert.Equal("Test group", result.GroupId);
        Assert.Equal(123, result.GroupUID);
        Assert.Equal("test-ukprn", result.UKPRN);
        Assert.Equal("Corpo", result.CompaniesHouseId);
        Assert.Equal(
            $"{stubAddress.AddressLine1}, {stubAddress.AddressLine2}, {stubAddress.Town}, {stubAddress.County}, {stubAddress.Postcode}",
            result.Address);
        Assert.Equal("mat", result.TypeName);
        Assert.Equal(input.GroupType.Code.Value, result.TypeCode);
        Assert.Equal("Closed on 10 October 2020", result.Status);

        Assert.Same(establishmentReadModels, result.Establishments);
        Assert.Same(memberReadModels, result.Members);
        Assert.Same(trusteeReadModels, result.Trustees);
    }

    [Fact]
    public void Map_Should_Map_Empty_Status_When_Status_Is_Null()
    {
        // Arrange
        GroupToGroupReadModelMapper sut = new(
            CreateMemberMapperMock().Object,
            CreateTrusteeMapperMock().Object,
            CreateEstablishmentMapperMock().Object);

        Group input = new GroupBuilder()
            .WithGroupStatus(statusLabel: null, effectiveDate: null)
            .Build();

        // Act
        GroupReadModel result = sut.Map(input);

        // Assert
        Assert.Equal(string.Empty, result.Status);
    }

    [Fact]
    public void Map_Should_Call_EstablishmentMapper()
    {
        // Arrange
        IReadOnlyCollection<Establishment> establishments =
            EstablishmentTestDoubles.Create(count: 10);

        Mock<IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>>> establishmentMapper =
            CreateEstablishmentMapperMock();

        GroupToGroupReadModelMapper sut = new(
            CreateMemberMapperMock().Object,
            CreateTrusteeMapperMock().Object,
            establishmentMapper.Object);

        Group input = new GroupBuilder()
            .WithEstablishments(establishments)
            .Build();

        // Act
        sut.Map(input);

        // Assert
        establishmentMapper.VerifyMapperCalledWith(establishments);
    }

    [Fact]
    public void Map_Should_Call_MemberMapper()
    {
        // Arrange
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(count: 10);

        Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>> memberMapper =
            CreateMemberMapperMock();

        GroupToGroupReadModelMapper sut = new(
            memberMapper.Object,
            CreateTrusteeMapperMock().Object,
            CreateEstablishmentMapperMock().Object);

        Group input = new GroupBuilder()
            .WithMembers(members)
            .Build();

        // Act
        sut.Map(input);

        // Assert
        memberMapper.VerifyMapperCalledWith(members);
    }

    [Fact]
    public void Map_Should_Call_TrusteeMapper()
    {
        // Arrange
        IReadOnlyCollection<Trustee> trustees = TrusteeTestDoubles.Create(count: 20);

        Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>> trusteeMapper =
            CreateTrusteeMapperMock();

        GroupToGroupReadModelMapper sut = new(
            CreateMemberMapperMock().Object,
            trusteeMapper.Object,
            CreateEstablishmentMapperMock().Object);

        Group input = new GroupBuilder()
            .WithTrustees(trustees)
            .Build();

        // Act
        sut.Map(input);

        // Assert
        trusteeMapper.VerifyMapperCalledWith(trustees);
    }

    private static Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>> CreateTrusteeMapperMock(
        TrusteeReadModel[]? mapOut = null)
    {
        return IMapperTestDouble.Map<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>(
            mapOut ?? []);
    }

    private static Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>> CreateMemberMapperMock(
        MemberReadModel[]? mapOut = null)
    {
        return IMapperTestDouble.Map<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>(
            mapOut ?? []);
    }

    private static Mock<IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>>> CreateEstablishmentMapperMock(
        EstablishmentReadModel[]? mapOut = null)
    {
        return IMapperTestDouble.Map<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>>(
            mapOut ?? []);
    }
}
