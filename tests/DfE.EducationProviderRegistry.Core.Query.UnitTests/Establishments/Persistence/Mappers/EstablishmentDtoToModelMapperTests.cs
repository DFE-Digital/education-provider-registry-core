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
    }

    [Fact]
    public void Map_WithSite_MapsAddress()
    {
        // Arrange
        Establishment dto = EstablishmentFactory.Create();

        Site site = dto.Site.First();

        EstablishmentToDetailsModelMapper mapper = new();

        // Act
        EstablishmentDetailsModel result = mapper.Map(dto);

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

        EstablishmentToDetailsModelMapper mapper = new();

        // Act
        EstablishmentDetailsModel result = mapper.Map(dto);

        // Assert
        Assert.NotNull(result.LocalAuthority);
        Assert.Equal(authority.AuthorityName, result.LocalAuthority.Name);
        Assert.Equal(authority.AuthorityCode, result.LocalAuthority.Code);
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
