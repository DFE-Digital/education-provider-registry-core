using System.Globalization;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentDtoToModelMapperTests
{
    [Fact]
    public void Map_WithValidDto_ReturnsMappedEstablishment()
    {
        // Arrange
        DateTime utcNow = DateTime.UtcNow;
        EstablishmentDto dto =
            new()
            {
                URN = "123456",
                UKPRN = "10000123",
                UPPN = "20000234",
                Name = "Test School",
                Number = "123",
                Status = "Open",
                Type = "Academy",
                PhaseOfEducation = "Primary",
                OpenDate = utcNow,
                ReasonEstablishmentOpened = "New school",
                CloseDate = null,
                ReasonEstablishmentClosed = null,
                Address = new AddressDto
                {
                    Street = "Street",
                    Town = "Town",
                    County = "County",
                    Postcode = "AB1 2CD"
                }
            };

        EstablishmentDtoToModelMapper mapper = new();

        // Act
        Establishment result = mapper.Map(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("123456", result.Urn.Value);
        Assert.Equal("123456", result.Urn.Value);
        Assert.Equal("10000123", result.Ukprn.Value);
        Assert.Equal("20000234", result.Uprn.Value);
        Assert.Equal("Test School", result.Name.Value);
        Assert.Equal("123", result.Number.Value);
        Assert.Equal("Open", result.Status.Value);
        Assert.Equal("Academy", result.Type.Value);
        Assert.Equal("Primary", result.Phase.Value);
        Assert.Equal(utcNow, result.OpenDate.Value);
        Assert.Equal("New school", result.ReasonEstablishmentOpened.Value);
        Assert.Null(result.CloseDate);
        Assert.Null(result.ReasonEstablishmentClosed);

        Assert.Equal("Street", result.Address?.Street);
        Assert.Equal("Town", result.Address?.Town);
        Assert.Equal("County", result.Address?.County);
        Assert.Equal("AB1 2CD", result.Address?.Postcode);
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
