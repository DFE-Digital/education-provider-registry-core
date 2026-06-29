using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentsDtoToModelMapperTests
{
    [Fact]
    public void Construct_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange
        Func<EstablishmentsToDetailsModelMapper> construct =
            () => new EstablishmentsToDetailsModelMapper(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_WithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        IMapper<Establishment, EstablishmentDetailsModel> innerMapper =
            MockTestDouble.Default<IMapper<Establishment, EstablishmentDetailsModel>>().Object;

        EstablishmentsToDetailsModelMapper sut = new(establishmentMapper: innerMapper);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.Map(null!));
    }

    [Fact]
    public void Map_WithValidDtos_MapsEachItemCorrectly()
    {
        // Arrange
        IReadOnlyCollection<Establishment> dtos =
            EstablishmentFactory.CreateMany(2);

        IReadOnlyCollection<EstablishmentDetailsModel> establishments =
            new EstablishmentCollectionBuilder()
                .WithCount(2)
                .Build();

        KeyValuePair<Establishment, EstablishmentDetailsModel>[] mappings =
        [
            new(dtos.ElementAt(0), establishments.ElementAt(index: 0)),
            new(dtos.ElementAt(1), establishments.ElementAt(index: 1))
        ];

        Mock<IMapper<Establishment, EstablishmentDetailsModel>> innerMapper =
            IMapperTestDouble.MapMany(mappings);

        EstablishmentsToDetailsModelMapper mapper =
            new(innerMapper.Object);

        // Act
        IReadOnlyCollection<EstablishmentDetailsModel> result = mapper.Map(dtos);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Same(mappings[0].Value, result.ElementAt(0));
        Assert.Same(mappings[1].Value, result.ElementAt(1));

        innerMapper.VerifyMapperCalled(count: 2);
        innerMapper.VerifyMapperCalledWith(mappings[0].Key, 1);
        innerMapper.VerifyMapperCalledWith(mappings[1].Key, 1);
    }

    [Fact]
    public void Map_WithEmptyCollection_ReturnsEmptyCollection()
    {
        // Arrange
        IReadOnlyCollection<Establishment> input = [];

        Mock<IMapper<Establishment, EstablishmentDetailsModel>> innerMapper =
            MockTestDouble.Default<
                IMapper<Establishment, EstablishmentDetailsModel>>();

        EstablishmentsToDetailsModelMapper sut = new(innerMapper.Object);

        // Act
        IReadOnlyCollection<EstablishmentDetailsModel> result = sut.Map(input);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        innerMapper.VerifyMapperCalled(count: 0);
    }
}
