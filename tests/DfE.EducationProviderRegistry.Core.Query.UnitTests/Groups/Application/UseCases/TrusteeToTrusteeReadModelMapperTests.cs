using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class TrusteeToTrusteeReadModelMapperTests
{
    [Fact]
    public void Map_Should_Return_Empty_When_Input_Is_Null()
    {
        // Arrange
        TrusteeToTrusteeReadModelMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeReadModel> result = sut.Map(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Return_Empty_When_Input_Is_Empty()
    {
        // Arrange
        TrusteeToTrusteeReadModelMapper sut = new();

        IEnumerable<Trustee> input = [];

        // Act
        IReadOnlyCollection<TrusteeReadModel> result = sut.Map(input);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Map_All_Properties()
    {
        // Arrange
        Trustee trustee = TrusteeTestDoubles.CreateWith(
            id: "1234567",
            name: "Test Trustee",
            role: "Chair of Trustees",
            startDate: new DateOnly(2024, 1, 1),
            endDate: new DateOnly(2028, 1, 1),
            title: "Mr",
            appointedBy: "Appointed by academy members");

        TrusteeToTrusteeReadModelMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeReadModel> result = sut.Map([trustee]);

        // Assert
        TrusteeReadModel readModel = Assert.Single(result);

        Assert.Equal(trustee.Id.Value, readModel.Id);
        Assert.Equal(trustee.Name.Value, readModel.FullName);
        Assert.Equal(trustee.Title, readModel.Title);
        Assert.Equal(trustee.Role, readModel.Role);
        Assert.Equal(trustee.AppointedBy, readModel.AppointedBy);
        Assert.Equal(trustee.StartDate, readModel.StartDate);
        Assert.Equal(trustee.EndDate, readModel.EndDate);
    }

    [Fact]
    public void Map_Should_Map_All_Items()
    {
        // Arrange
        IReadOnlyCollection<Trustee> trustees =
            TrusteeTestDoubles.Create(30);

        TrusteeToTrusteeReadModelMapper sut = new();

        // Act
        IReadOnlyCollection<TrusteeReadModel> result =
            sut.Map(trustees);

        // Assert
        Assert.Equal(trustees.Count, result.Count);

        Assert.Equal(
            trustees.Select(trustee => trustee.Id.Value),
            result.Select(readModel => readModel.Id));
    }
}
