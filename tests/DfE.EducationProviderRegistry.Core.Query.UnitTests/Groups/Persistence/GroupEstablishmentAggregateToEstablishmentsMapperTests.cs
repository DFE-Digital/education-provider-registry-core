using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Persistence;

public sealed class GroupEstablishmentAggregateToEstablishmentsMapperTests
{
    [Fact]
    public void Map_WhenInputIsNull_ReturnsEmptyCollection()
    {
        // Arrange
        GroupEstablishmentAggregateToEstablishmentsMapper sut = new();

        // Act
        IReadOnlyCollection<Establishment> result = sut.Map(null!);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_WhenInputIsEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        GroupEstablishmentAggregateToEstablishmentsMapper sut = new();

        GroupEstablishmentAggregate[] input = [];

        // Act
        IReadOnlyCollection<Establishment> result = sut.Map(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_WhenInputContainsEstablishments_MapsEstablishments()
    {
        // Arrange
        GroupEstablishmentAggregateToEstablishmentsMapper sut = new();

        GroupEstablishmentAggregate[] input =
        [
            new()
            {
                EstablishmentUrn = "107809",
                EstablishmentName = "Guiseley Primary School"
            },
            new()
            {
                EstablishmentUrn = "107810",
                EstablishmentName = "Rawdon Littlemoor Primary School"
            }
        ];

        // Act
        IReadOnlyCollection<Establishment> result = sut.Map(input);

        // Assert
        Assert.Collection(
            result,
            establishment =>
            {
                Assert.Equal("107809", establishment.Urn.Value);
                Assert.Equal("Guiseley Primary School", establishment.Name.Value);
            },
            establishment =>
            {
                Assert.Equal("107810", establishment.Urn.Value);
                Assert.Equal("Rawdon Littlemoor Primary School", establishment.Name.Value);
            });
    }
}
