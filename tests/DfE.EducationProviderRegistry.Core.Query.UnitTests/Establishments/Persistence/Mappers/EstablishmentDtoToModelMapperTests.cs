using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentDtoToModelMapperTests
{
    private readonly EstablishmentToDetailsModelMapper _mapper = new();

    [Fact]
    public void Map_WithValidDto_ReturnsMappedEstablishment()
    {
        // Arrange
        Establishment dto = EstablishmentFactory.Create();

        // Act
        EstablishmentDetailsModel result = _mapper.Map(dto);

        // Assert
        Assert.Equal(dto.Urn, result.Urn.Value);
        Assert.Equal(dto.Name, result.Name.Value);
        Assert.Equal(dto.EstablishmentNumber, result.Number.Value);
        Assert.Equal(dto.EstablishmentStatus.Name, result.Status.Value);
        Assert.Equal(dto.EstablishmentType.Name, result.Type.Value);
        Assert.Equal(dto.EstablishmentProvision?.EducationPhase?.Name, result.Phase.Value);
        Assert.Equal(dto.Uid, result.Uid);
        Assert.Equal(dto.EstablishmentGroupMembership.FirstOrDefault()?.Group.Name, result.GroupName);
        Assert.Equal(dto.EstablishmentGroupMembership.FirstOrDefault()?.Group.GroupType.Name, result.GroupType);
        Assert.Equal(dto.EstablishmentGroupMembership.FirstOrDefault()?.StartDate, result.GroupOpenDate);
        Assert.Equal(dto.Contact.FirstOrDefault()?.Website, result.ContactDetails?.Website);
        Assert.Equal(dto.Contact.FirstOrDefault()?.TelephoneNumber, result.ContactDetails?.TelephoneNumber);

    }

    [Fact]
    public void Map_WithSite_MapsAddress()
    {
        // Arrange
        Establishment dto = EstablishmentFactory.Create();

        Site site = dto.Site.First();

        // Act
        EstablishmentDetailsModel result = _mapper.Map(dto);

        // Assert
        Assert.NotNull(result.Address);
        Assert.Equal(site.Name ?? string.Empty, result.Address.Name);
        Assert.Equal(site.AddressLine1 ?? string.Empty, result.Address.AddressLine1);
        Assert.Equal(site.AddressLine2 ?? string.Empty, result.Address.AddressLine2);
        Assert.Equal(site.Town ?? string.Empty, result.Address.Town);
        Assert.Equal(site.County ?? string.Empty, result.Address.County);
        Assert.Equal(site.Postcode ?? string.Empty, result.Address.Postcode);

    }

    [Fact]
    public void Map_WithAuthority_MapsLocalAuthority()
    {
        // Arrange
        Establishment dto = EstablishmentFactory.Create();

        EstablishmentAuthority authority = dto.EstablishmentAuthority.First();

        // Act
        EstablishmentDetailsModel result = _mapper.Map(dto);

        // Assert
        Assert.NotNull(result.LocalAuthority);
        Assert.Equal(authority.AuthorityName, result.LocalAuthority.Name);
        Assert.Equal(authority.AuthorityCode, result.LocalAuthority.Code);
    }

    [Fact]
    public void Map_WithAuthority_MapsNullNonRequiredValues()
    {
        // Arrange
        Establishment input = new()
        {
            Urn = "12345",
            Name = "TestName",
            EstablishmentStatus = new EstablishmentStatus { Name = "testStatus"},
            EstablishmentType = new EstablishmentType { Name = "testType"},
            EstablishmentProvision = new EstablishmentProvision { EducationPhase = new EducationPhase() },
            EstablishmentGroupMembership = []
        };

        // Act
        EstablishmentDetailsModel result = _mapper.Map(input);

        // Assert
        Assert.Null(result.GroupName);
        Assert.Null(result.GroupType);
        Assert.Null(result.GroupOpenDate);
        Assert.Null(result.LocalAuthority);
        Assert.Null(result.AgeRange);
        Assert.Null(result.ReligiousCharacter);
        Assert.Null(result.Address);
        Assert.Null(result.ContactDetails);
        Assert.Null(result.Headteacher);
        Assert.Null(result.SenProvision);
        Assert.Null(result.Headteacher);
        Assert.Null(result.Phase.Value);
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
