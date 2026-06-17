using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class MemberToMemberDtoMapperTests
{
    [Fact]
    public void Map_Should_Return_Empty_When_Input_Is_Null()
    {
        // Arrange
        MemberToMemberDtoMapper sut = new();

        // Act
        IReadOnlyCollection<MemberDto> result = sut.Map(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Return_Empty_When_Input_Is_Empty()
    {
        // Arrange
        MemberToMemberDtoMapper sut = new();

        IEnumerable<Member> input = [];

        // Act
        IReadOnlyCollection<MemberDto> result = sut.Map(input);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Map_All_Properties()
    {
        // Arrange
        Member member = MemberTestDoubles.Create(1).Single();

        MemberToMemberDtoMapper sut = new();

        // Act
        IReadOnlyCollection<MemberDto> result = sut.Map(new[] { member });

        MemberDto dto = Assert.Single(result);

        // Assert
        Assert.Equal(member.Id.Value, dto.Identifier);
        Assert.Equal(member.Name.Value, dto.FullName);
        Assert.Equal(member.StartDate, dto.StartDate);
    }

    [Fact]
    public void Map_Should_Order_By_StartDate_Descending()
    {
        // Arrange
        Member older = MemberTestDoubles.CreateWith(startDate: new DateTime(2020, 1, 1));
        Member newer = MemberTestDoubles.CreateWith(startDate: new DateTime(2025, 1, 1));

        MemberToMemberDtoMapper sut = new();

        // Act
        IReadOnlyCollection<MemberDto> result = sut.Map(new[] { older, newer });

        // Assert
        Assert.Equal(newer.Id.Value, result.ToArray()[0].Identifier);
        Assert.Equal(older.Id.Value, result.ToArray()[1].Identifier);
    }

    [Fact]
    public void Map_Should_Map_All_Items()
    {
        // Arrange
        IReadOnlyCollection<Member> members = MemberTestDoubles.Create(50);

        MemberToMemberDtoMapper sut = new();

        // Act
        IReadOnlyCollection<MemberDto> result = sut.Map(members);

        // Assert
        Assert.Equal(members.Count, result.Count);

        Assert.Equivalent(
            expected: members.OrderByDescending(t => t.StartDate).Select(t => t.Id.Value),
            actual: result.Select(t => t.Identifier));
    }
}
