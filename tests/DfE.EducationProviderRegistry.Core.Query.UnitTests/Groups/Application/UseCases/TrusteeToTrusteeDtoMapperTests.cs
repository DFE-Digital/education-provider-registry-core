using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class TrusteeToTrusteeDtoMapperTests
{
    [Fact]
    public void Map_Should_Return_Empty_When_Input_Is_Null()
    {
        // Arrange
        TrusteeToTrusteeDtoMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeDto> result = sut.Map(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Return_Empty_When_Input_Is_Empty()
    {
        // Arrange
        TrusteeToTrusteeDtoMapper sut = new();

        IEnumerable<Trustee> input = [];

        // Act
        IReadOnlyCollection<TrusteeDto> result = sut.Map(input);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Map_All_Properties()
    {
        // Arrange
        Trustee trustee = TrusteeTestDoubles.Create(1).Single();

        TrusteeToTrusteeDtoMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeDto> result = sut.Map(new[] { trustee });

        TrusteeDto dto = Assert.Single(result);

        // Assert
        Assert.Equal(trustee.Id.Value, dto.Id);
        Assert.Equal(trustee.Name.FullName, dto.FullName);
        Assert.Equal(trustee.StartDate, dto.StartDate);
        Assert.Equal(trustee.Title?.Type, dto.Title);
    }

    [Fact]
    public void Map_Should_Order_By_Title_Priority()
    {
        // Arrange
        Trustee chair = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.Chair);
        Trustee cfo = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.CFO);
        Trustee accounting = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.AccountingOfficer);
        Trustee other = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.Other);

        TrusteeToTrusteeDtoMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeDto> result = sut.Map(new[] { other, cfo, chair, accounting });

        TrusteeDto[] ordered = [.. result];

        // Assert
        Assert.Equal(TrusteeTitleType.Chair, ordered[0].Title);
        Assert.Equal(TrusteeTitleType.CFO, ordered[1].Title);
        Assert.Equal(TrusteeTitleType.AccountingOfficer, ordered[2].Title);
        Assert.Equal(TrusteeTitleType.Other, ordered[3].Title);
    }

    [Fact]
    public void Map_Should_Then_Order_By_StartDate_Descending_When_Same_Title()
    {
        // Arrange
        Trustee older = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.CFO, startDate: new DateTime(2020, 1, 1));
        Trustee newer = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.CFO, startDate: new DateTime(2025, 1, 1));

        TrusteeToTrusteeDtoMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeDto> result = sut.Map(new[] { older, newer });

        TrusteeDto[] ordered = [.. result];

        // Assert
        Assert.Equal(newer.Id.Value, ordered[0].Id);
        Assert.Equal(older.Id.Value, ordered[1].Id);
    }

    [Fact]
    public void Map_Should_Map_All_Items()
    {
        // Arrange
        Trustee chair = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.Chair);
        Trustee cfo = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.CFO);
        Trustee accounting = TrusteeTestDoubles.CreateWith(titleType: TrusteeTitleType.AccountingOfficer);
        IReadOnlyCollection<Trustee> otherTrustees = TrusteeTestDoubles.Create(30);

        IReadOnlyCollection<Trustee> inputMapTrustees = [.. otherTrustees, chair, cfo, accounting];

        TrusteeToTrusteeDtoMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeDto> result = sut.Map(inputMapTrustees);

        // Assert
        Assert.Equal(inputMapTrustees.Count, result.Count);


        Trustee[] expected =
            [..inputMapTrustees
                .OrderBy((trustee) => trustee.Title?.Type switch
                {
                    TrusteeTitleType.Chair => 0,
                    TrusteeTitleType.CFO => 1,
                    TrusteeTitleType.AccountingOfficer => 2,
                    _ => 99
                }).ThenByDescending(t => t.StartDate)
            ];

        Assert.Equal(
            expected: expected.Select(t => t.Id.Value),
            actual: result.Select(t => t.Id));
    }
}
