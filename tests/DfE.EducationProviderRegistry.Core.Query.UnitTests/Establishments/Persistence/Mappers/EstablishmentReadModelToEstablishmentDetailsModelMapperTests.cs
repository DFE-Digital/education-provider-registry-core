using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentReadModelToEstablishmentDetailsModelMapperTests
{
    private readonly EstablishmentReadModelToEstablishmentDetailsModelMapper _mapper = new();


    [Fact]
    public void Map_WithNullDto_ThrowsArgumentNullException()
    {
        // Arrange
        EstablishmentReadModelToEstablishmentDetailsModelMapper mapper = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_WithValidDto_ReturnsMappedEstablishment()
    {
        // Arrange
        EstablishmentAggregate establishment = EstablishmentAggregateTestDoubles.Stub();

        List<EstablishmentGovernorAggregate> governors =
        [
            new()
        {
            GovernorId = "123",
            GovernorName = "Joe Bloggs"
        }
        ];

        EstablishmentReadModel input =
            new(establishment, governors);

        // Act
        EstablishmentDetailsModel result = _mapper.Map(input);

        // Assert
        Assert.Equal(establishment.Urn, result.Identifiers.Urn);
        Assert.Equal(establishment.Name, result.Name?.Value);
        Assert.Equal(establishment.LocalAuthorityCode + "/" + establishment.EstablishmentNumber, result.Identifiers?.DfENumber);
        Assert.Equal(establishment.LocalAuthorityCode + establishment.EstablishmentNumber, result.Identifiers?.LaEstab);
        Assert.Equal(establishment.StatusCode!.Value, (int)result.Status!.Status);
        Assert.Equal(establishment.EstablishmentTypeName, result.Type?.Value);
        Assert.Equal(establishment.EducationPhaseName, result.Phase?.Value);

        Assert.Equal(establishment.GroupName, result.Group?.GroupName);
        Assert.Equal(establishment.GroupCode, result.Group?.Code);
        Assert.Equal(establishment.GroupTypeName, result.GroupType);
        Assert.Equal(establishment.GroupOpenDate, result.GroupOpenDate);

        Assert.Equal(establishment.Website, result.ContactDetails?.Website);
        Assert.Equal(establishment.TelephoneNumber, result.ContactDetails?.TelephoneNumber);
    }

    [Fact]
    public void Map_WithAddress_MapsAddress()
    {
        // Arrange
        EstablishmentAggregate establishment = EstablishmentAggregateTestDoubles.Stub();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result = _mapper.Map(input);

        // Assert
        Assert.NotNull(result.Address);

        Assert.Equal(establishment.SiteName, result.Address.Name);
        Assert.Equal(establishment.AddressLine1, result.Address.AddressLine1);
        Assert.Equal(establishment.AddressLine2, result.Address.AddressLine2);
        Assert.Equal(establishment.Town, result.Address.Town);
        Assert.Equal(establishment.County, result.Address.County);
        Assert.Equal(establishment.Postcode, result.Address.Postcode);
    }

    [Fact]
    public void Map_WithLocalAuthority_MapsLocalAuthority()
    {
        // Arrange
        EstablishmentAggregate establishment = EstablishmentAggregateTestDoubles.Stub();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result = _mapper.Map(input);

        // Assert
        Assert.NotNull(result.LocalAuthority);

        Assert.Equal(
            establishment.LocalAuthorityName,
            result.LocalAuthority.Name);

        Assert.Equal(
            establishment.LocalAuthorityCode!.Value,
            result.LocalAuthority.Code);
    }

    [Fact]
    public void Map_WithGovernors_MapsGovernors()
    {
        // Arrange
        List<EstablishmentGovernorAggregate> governors =
        [
            new()
        {
            GovernorId = "123",
            GovernorName = "Governor One"
        },
        new()
        {
            GovernorId = "456",
            GovernorName = "Governor Two"
        }
        ];

        EstablishmentReadModel input =
            new(
                EstablishmentAggregateTestDoubles.Stub(),
                governors);

        // Act
        EstablishmentDetailsModel result = _mapper.Map(input);

        // Assert
        Assert.Equal(2, result.Governors!.Count());

        Assert.Contains(
            result.Governors!,
            g => g.Identifier.Value == "123");

        Assert.Contains(
            result.Governors!,
            g => g.Identifier.Value == "456");
    }

    [Fact]
    public void Map_WithNullOptionalValues_MapsNullOptionalValues()
    {
        // Arrange
        EstablishmentAggregate establishment = new()
        {
            Urn = "12345",
            Name = "Test School"
        };

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result = _mapper.Map(input);

        // Assert
        Assert.Null(result.Group);
        Assert.Null(result.GroupType);
        Assert.Null(result.GroupOpenDate);
        Assert.Null(result.LocalAuthority);
        Assert.Null(result.Address);
        Assert.Null(result.ContactDetails);
        Assert.Null(result.Headteacher);
        Assert.Null(result.SenProvision);
        Assert.Null(result.Phase);
        Assert.Null(result.AgeRange);
        Assert.Null(result.ReligiousCharacter);
    }
}
