using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentDtoToModelMapperTests
{
    [Fact]
    public void Map_WithValidDto_ReturnsMappedEstablishment()
    {
        // Arrange
        Establishment dto = EstablishmentFactory.Create();

        EstablishmentToDetailsModelMapper mapper = new();

        // Act
        EstablishmentDetailsModel result = mapper.Map(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Urn, result.Urn.Value);
        Assert.Equal(dto.Name, result.Name.Value);
        Assert.Equal(dto.EstablishmentNumber, result.Number.Value);
    }

    [Fact]
    public void Map_WithNullDto_ThrowsArgumentNullException()
    {
        // Arrange
        EstablishmentToDetailsModelMapper mapper = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }
}
