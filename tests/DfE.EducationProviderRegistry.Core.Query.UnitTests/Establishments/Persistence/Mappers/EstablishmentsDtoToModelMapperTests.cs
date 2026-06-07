using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using Moq;
using Tests.Shared;
using Tests.Shared.Mapper;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentsDtoToModelMapperTests
{
    [Fact]
    public void Construct_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange
        Func<EstablishmentsDtoToModelMapper> construct =
            () => new EstablishmentsDtoToModelMapper(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Map_WithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        IMapper<EstablishmentDataTransferObject, Establishment> innerMapper =
            MockTestDouble.Default<IMapper<EstablishmentDataTransferObject, Establishment>>().Object;

        EstablishmentsDtoToModelMapper sut = new(establishmentMapper: innerMapper);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.Map(null!));
    }

    [Fact]
    public void Map_WithValidDtos_MapsEachItemCorrectly()
    {
        // Arrange
        IReadOnlyCollection<EstablishmentDataTransferObject> dtos =
            new EstablishmentDataTransferObjectBuilder().BuildMany(2);

        IReadOnlyCollection<Establishment> establishments =
            new EstablishmentCollectionBuilder()
                .WithCount(2)
                .Build();

        KeyValuePair<EstablishmentDataTransferObject, Establishment>[] mappings =
        [
            new(dtos.ElementAt(0), establishments.ElementAt(index: 0)),
            new(dtos.ElementAt(1), establishments.ElementAt(index: 1))
        ];

        Mock<IMapper<EstablishmentDataTransferObject, Establishment>> innerMapper =
            IMapperTestDouble.MapMany(mappings);

        EstablishmentsDtoToModelMapper mapper =
            new(innerMapper.Object);

        // Act
        IReadOnlyCollection<Establishment> result = mapper.Map(dtos);

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
        IReadOnlyCollection<EstablishmentDataTransferObject> input = [];

        Mock<IMapper<EstablishmentDataTransferObject, Establishment>> innerMapper =
            MockTestDouble.Default<
                IMapper<EstablishmentDataTransferObject, Establishment>>();

        EstablishmentsDtoToModelMapper sut = new(innerMapper.Object);

        // Act
        IReadOnlyCollection<Establishment> result = sut.Map(input);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        innerMapper.VerifyMapperCalled(count: 0);
    }
}
