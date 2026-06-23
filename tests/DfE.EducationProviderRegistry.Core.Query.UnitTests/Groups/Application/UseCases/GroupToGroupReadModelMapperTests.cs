using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;
using Moq;
using Address = DfE.EducationProviderRegistry.Core.Query.Shared.Address;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class GroupToGroupReadModelMapperTests
{
    private static Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>> CreateTrusteeMapperMock(TrusteeReadModel[]? mapOut = null)
    {
        return IMapperTestDouble.Map<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>(mapOut ?? []);
    }

    private static Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>> CreateMemberMapperMock(MemberReadModel[]? mapOut = null)
    {
        return IMapperTestDouble.Map<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>(mapOut ?? []);
    }



    [Fact]
    public void Constructor_Should_Throw_When_MemberMapper_Is_Null()
    {
        // Arrange
        Func<GroupToGroupReadModelMapper> construct = () => new(null!, CreateTrusteeMapperMock().Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_TrusteeMapper_Is_Null()
    {
        // Arrange

        Func<GroupToGroupReadModelMapper> construct = () => new(CreateMemberMapperMock().Object, null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_Should_Throw_When_Input_Is_Null()
    {
        // Arrange
        GroupToGroupReadModelMapper sut = new(CreateMemberMapperMock().Object, CreateTrusteeMapperMock().Object);

        Func<GroupReadModel> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }


    [Fact]
    public void Map_Should_Map_All_Properties()
    {
        // Arrange
        MemberReadModel[] memberDtos = [];
        TrusteeReadModel[] trusteeDtos = [];

        Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>> memberMapper = CreateMemberMapperMock(memberDtos);

        Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>> trusteeMapper = CreateTrusteeMapperMock(trusteeDtos);

        GroupToGroupReadModelMapper sut = new(memberMapper.Object, trusteeMapper.Object);

        Address stubAddress = AddressTestDoubles.Generate();

        Group input = new GroupBuilder()
            .WithName("Test Group Name")
            .WithGroupId("Test group")
            .WithGroupUid(123)
            .WithCompaniesHouseId("Corpo")
            .WithUkprn("test-ukprn")
            .WithAddress(stubAddress.Street, stubAddress.Town, stubAddress.County, stubAddress.Postcode)
            .WithGroupStatus(GroupOpenState.Closed, new(2020, 10, 10))
            .WithType("mat")
            .WithAcademies(AcademyTestDouble.Create(3))
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
        Assert.Equal($"{stubAddress.Street}, {stubAddress.Town}, {stubAddress.County}, {stubAddress.Postcode}", result.Address);
        Assert.Equal("mat", result.Type);
        Assert.Equal("Closed on 10 October 2020", result.Status);

        Assert.Same(memberDtos, result.Members);
        Assert.Same(trusteeDtos, result.Trustees);

        Assert.NotEmpty(result.Academies);

        IEnumerable<Academy> expectaAcademiesSortedByNameAsc = input.Academies.OrderBy(t => t.Name.ToString());

        Assert.Equivalent(
            expectaAcademiesSortedByNameAsc,
            result.Academies);
    }


    [Fact]
    public void Map_Should_Call_MemberMapper()
    {
        // Arrange
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(count: 10);

        Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>> memberMapper = CreateMemberMapperMock();

        GroupToGroupReadModelMapper sut = new(memberMapper.Object, CreateTrusteeMapperMock().Object);

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

        Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>> trusteeMapperMock = CreateTrusteeMapperMock();

        GroupToGroupReadModelMapper sut = new(CreateMemberMapperMock().Object, trusteeMapperMock.Object);

        Group input = new GroupBuilder()
            .WithTrustees(trustees)
            .Build();

        // Act
        sut.Map(input);

        // Assert
        trusteeMapperMock.VerifyMapperCalledWith(trustees);
    }
}
