using System.Globalization;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentDtoToModelMapperTests
{
    [Fact]
    public void Map_WithValidDto_ReturnsMappedEstablishment()
    {
        // Arrange
        EstablishmentDto dto = EstablishmentDtoFactory.Create();

        EstablishmentDtoToModelMapper mapper = new();

        // Act
        Establishment result = mapper.Map(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.URN, result.Urn.Value);
        Assert.Equal(dto.UKPRN, result.Ukprn.Value);
        Assert.Equal(dto.UPPN, result.Uprn.Value);
        Assert.Equal(dto.Name, result.Name.Value);
        Assert.Equal(dto.Number, result.Number.Value);
        Assert.Equal(dto.Status, result.Status.Value);
        Assert.Equal(dto.Type, result.Type.Value);
        Assert.Equal(dto.PhaseOfEducation, result.Phase.Value);
        Assert.Equal(dto.OpenDate, result.OpenDate.Value);
        Assert.Equal(dto.ReasonEstablishmentOpened, result.ReasonEstablishmentOpened.Value);
        Assert.Null(result.CloseDate);
        Assert.Null(result.ReasonEstablishmentClosed);

        Assert.Equal(dto.Address?.Street, result.Address?.Street);
        Assert.Equal(dto.Address?.Town, result.Address?.Town);
        Assert.Equal(dto.Address?.County, result.Address?.County);
        Assert.Equal(dto.Address?.Postcode, result.Address?.Postcode);
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
