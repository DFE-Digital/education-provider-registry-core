using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Mappers;

public sealed class EstablishmentToSearchResultMapperTests
{
    private static Establishment BuildValidEstablishment()
    {
        Site site =
            new()
            {
                AddressLine1 = "123 Street",
                Town = "Townsville",
                County = "Countyshire",
                Postcode = "AB1 2CD"
            };

        EstablishmentGroupMembership membership =
            new()
            {
                Group = new GroupRecord
                {
                    Name = "Group Name",
                    Code = "GRP123"
                }
            };

        EstablishmentAuthority authority =
            new()
            {
                AuthorityCode = "LA001",
                AuthorityName = "Local Authority"
            };

        Data.DatabaseModels.Models.EstablishmentType type = new() { Name = "Academy" };

        Establishment establishment =
            new()
            {
                Urn = "123456",
                Name = "Test School",
                EstablishmentType = type,
                Site = [site],
                EstablishmentGroupMembership = [membership],
                EstablishmentAuthority = [authority]
            };

        return establishment;
    }

    [Fact]
    public void Map_Throws_WhenInputIsNull()
    {
        // arrange
        EstablishmentToSearchResultMapper mapper = new();

        // act // assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_ReturnsExpectedResult_WhenInputIsValid()
    {
        // arrange
        Establishment establishment = BuildValidEstablishment();
        EstablishmentToSearchResultMapper mapper = new();

        // act
        EstablishmentSearchResult result = mapper.Map(establishment);

        // assert
        Assert.Equal("123456", result.Urn.Value);
        Assert.Equal("Test School", result.Name.Value);
        Assert.Equal("123 Street", result.Address?.Street);
        Assert.Equal("Townsville", result.Address?.Town);
        Assert.Equal("Countyshire", result.Address?.County);
        Assert.Equal("AB1 2CD", result.Address?.Postcode);
        Assert.Equal("Academy", result.Type?.Value);
        Assert.Equal("Group Name", result.Group?.PartOfName);
        Assert.Equal("GRP123", result.Group?.PartOfCode);
        Assert.Equal("LA001", result.LocalAuthority?.Code);
        Assert.Equal("Local Authority", result.LocalAuthority?.Name);
    }
}
