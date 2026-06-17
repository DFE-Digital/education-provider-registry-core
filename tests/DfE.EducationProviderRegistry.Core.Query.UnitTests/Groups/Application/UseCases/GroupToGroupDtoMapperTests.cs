using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using Moq;
using Tests.Shared.Mapper;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class GroupToGroupDtoMapperTests
{
    private static Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>>> CreateTrusteeMapperMock(TrusteeDto[]? mapOut = null)
    {
        return IMapperTestDouble.Map<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>>(mapOut ?? []);
    }

    private static Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberDto>>> CreateMemberMapperMock(MemberDto[]? mapOut = null)
    {
        return IMapperTestDouble.Map<IEnumerable<Member>, IReadOnlyCollection<MemberDto>>(mapOut ?? []);
    }



    [Fact]
    public void Constructor_Should_Throw_When_MemberMapper_Is_Null()
    {
        // Arrange
        Func<GroupToGroupDtoMapper> construct = () => new(null!, CreateTrusteeMapperMock().Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_TrusteeMapper_Is_Null()
    {
        // Arrange

        Func<GroupToGroupDtoMapper> construct = () => new(CreateMemberMapperMock().Object, null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_Should_Throw_When_Input_Is_Null()
    {
        // Arrange
        GroupToGroupDtoMapper sut = new(CreateMemberMapperMock().Object, CreateTrusteeMapperMock().Object);

        Func<GroupDto> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }


    [Fact]
    public void Map_Should_Map_All_Properties()
    {
        // Arrange
        MemberDto[] memberDtos = [];
        TrusteeDto[] trusteeDtos = [];

        Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberDto>>> memberMapper = CreateMemberMapperMock(memberDtos);

        Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>>> trusteeMapper = CreateTrusteeMapperMock(trusteeDtos);

        GroupToGroupDtoMapper sut = new(memberMapper.Object, trusteeMapper.Object);

        Group input = new GroupBuilder()
            .WithGroupId("Test group")
            .WithGroupUid(123)
            .WithCompaniesHouseId("Corpo")
            .WithAcademies(AcademyTestDouble.Create(3))
            .WithMembers(MemberTestDoubles.Create(5))
            .WithTrustees(TrusteeTestDoubles.Create(7))
            .Build();

        // Act
        GroupDto result = sut.Map(input);

        // Assert
        Assert.Equal("Test group", result.GroupId);
        Assert.Equal(123, result.GroupUID);
        Assert.Equal("Corpo", result.CompaniesHouseId);

        Assert.Same(memberDtos, result.Members);
        Assert.Same(trusteeDtos, result.Trustees);

        Assert.NotEmpty(result.Academies);

        IEnumerable<Academy> expectaAcademiesSortedByNameAsc = input.Academies.OrderBy(t => t.Name.Value);
        Assert.Equivalent(
            expectaAcademiesSortedByNameAsc,
            result.Academies);
    }


    [Fact]
    public void Map_Should_Call_MemberMapper()
    {
        // Arrange
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(count: 10);

        Mock<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberDto>>> memberMapper = CreateMemberMapperMock();

        GroupToGroupDtoMapper sut = new(memberMapper.Object, CreateTrusteeMapperMock().Object);

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

        Mock<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>>> trusteeMapperMock = CreateTrusteeMapperMock();

        GroupToGroupDtoMapper sut = new(CreateMemberMapperMock().Object, trusteeMapperMock.Object);

        Group input = new GroupBuilder()
            .WithTrustees(trustees)
            .Build();

        // Act
        sut.Map(input);

        // Assert
        trusteeMapperMock.VerifyMapperCalledWith(trustees);
    }
}
