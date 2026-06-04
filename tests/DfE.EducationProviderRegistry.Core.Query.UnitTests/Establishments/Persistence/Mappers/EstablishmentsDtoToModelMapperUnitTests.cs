using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers.TestDoubles.StubBuilders;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentsDtoToModelMapperUnitTests
{
    [Fact]
    public void Construct_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Func<EstablishmentsDtoToModelMapper> construct = () => new EstablishmentsDtoToModelMapper(null!);
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_WithValidDtos_MapsEachItemCorrectly()
    {
        // Arrange
        IReadOnlyCollection<EstablishmentDataTransferObject> dtos =
            new EstablishmentDataTransferObjectBuilder()
                .BuildMany(2);

        EstablishmentDtoToModelMapper innerMapper = new();
        EstablishmentsDtoToModelMapper mapper = new(innerMapper);

        // Act
        IReadOnlyCollection<Establishment> result = mapper.Map(dtos);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Collection(
            result,
            establishment => Assert.Equal(dtos.ElementAt(0).URN, establishment.Identifier.Urn),
            establishment => Assert.Equal(dtos.ElementAt(1).URN, establishment.Identifier.Urn));
    }

    [Fact]
    public void Map_WithEmptyCollection_ReturnsEmptyCollection()
    {
        // Arrange
        List<EstablishmentDataTransferObject> input = [];
        EstablishmentDtoToModelMapper innerMapper = new();
        EstablishmentsDtoToModelMapper mapper = new(innerMapper);

        // Act
        IReadOnlyCollection<Establishment> result = mapper.Map(input);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Map_WithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        EstablishmentDtoToModelMapper innerMapper = new();
        EstablishmentsDtoToModelMapper mapper = new(innerMapper);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_UsesOnlyTheFilledPortionOfTheRentedBuffer()
    {
        // Arrange
        EstablishmentDataTransferObject dto =
            new EstablishmentDataTransferObjectBuilder()
                .WithUrn("999999")
                .Build();

        List<EstablishmentDataTransferObject> input = [dto];
        EstablishmentDtoToModelMapper innerMapper = new();
        EstablishmentsDtoToModelMapper mapper = new(innerMapper);

        // Act
        IReadOnlyCollection<Establishment> result = mapper.Map(input);

        // Assert
        Assert.Single(result);
        Assert.Equal("999999", result.First().Identifier.Urn);
    }
}
