using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Persistence;

public sealed class EstablishmentsToEstablishmentReadModelMapperTests
{
    [Fact]
    public void Map_WhenInputIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        EstablishmentsToEstablishmentReadModelMapper sut = new();

        Func<IReadOnlyCollection<EstablishmentReadModel>> act = () =>
            sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_WhenInputIsEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        EstablishmentsToEstablishmentReadModelMapper sut = new();

        Establishment[] input = [];

        // Act
        IReadOnlyCollection<EstablishmentReadModel> result = sut.Map(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_Should_Map_Establishment_Properties()
    {
        // Arrange
        EstablishmentsToEstablishmentReadModelMapper sut = new();

        Establishment[] input =
        [
            new(
                new("107809"),
                new("Guiseley Primary School"))
        ];

        // Act
        IReadOnlyCollection<EstablishmentReadModel> result = sut.Map(input);

        // Assert
        EstablishmentReadModel establishment = Assert.Single(result);

        Assert.Equal("107809", establishment.Urn);
        Assert.Equal("Guiseley Primary School", establishment.Name);
    }

    [Fact]
    public void Map_Should_Order_Establishments_By_Name()
    {
        // Arrange
        EstablishmentsToEstablishmentReadModelMapper sut = new();

        Establishment[] input =
        [
            new(
                new("134317"),
                new("Rufford Park Primary School")),
            new(
                new("107809"),
                new("Guiseley Primary School")),
            new(
                new("107810"),
                new("Rawdon Littlemoor Primary School"))
        ];

        // Act
        IReadOnlyCollection<EstablishmentReadModel> result = sut.Map(input);

        // Assert
        Assert.Collection(
            result,
            establishment =>
            {
                Assert.Equal("107809", establishment.Urn);
                Assert.Equal("Guiseley Primary School", establishment.Name);
            },
            establishment =>
            {
                Assert.Equal("107810", establishment.Urn);
                Assert.Equal("Rawdon Littlemoor Primary School", establishment.Name);
            },
            establishment =>
            {
                Assert.Equal("134317", establishment.Urn);
                Assert.Equal("Rufford Park Primary School", establishment.Name);
            });
    }
}
