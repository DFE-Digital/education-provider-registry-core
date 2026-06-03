using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentDtoToModelMapperUnitTests
{
    [Fact]
    public void Map_WithValidDto_ReturnsMappedEstablishment()
    {
        // Arrange
        EstablishmentDataTransferObject dto =
            new()
            {
                URN = "123456"
            };

        EstablishmentDtoToModelMapper mapper = new();

        // Act
        Establishment result = mapper.Map(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123456", result.Identifier.Urn);
    }

    [Fact]
    public void Map_WithNullDto_ThrowsArgumentNullException()
    {
        // Arrange
        EstablishmentDtoToModelMapper mapper = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }
}
