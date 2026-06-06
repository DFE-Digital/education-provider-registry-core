using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence;

public sealed class EstablishmentsRepositoryTests
{
    private readonly Mock<IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>> _collectionMapper;
    private readonly IReadOnlyCollection<Establishment> _mappedResponseDtos;
    private readonly EstablishmentsRepository _sut;

    public EstablishmentsRepositoryTests()
    {
        _mappedResponseDtos =
            new EstablishmentCollectionBuilder()
                .WithCount(100)
                .Build();

        _collectionMapper =
            IMapperTestDouble.For<
                IEnumerable<EstablishmentDataTransferObject>,
                IReadOnlyCollection<Establishment>>(
                    output: _mappedResponseDtos);
        
        _sut = new EstablishmentsRepository(_collectionMapper.Object);
    }

    [Fact]
    public async Task GetEstablishments_ShouldMapDtosGeneratedByBuilder()
    {
        // Arrange Act
        IReadOnlyCollection<Establishment> result =
            await _sut.GetEstablishments(CancellationToken.None);

        // Assert
        _collectionMapper.Verify(
            (mapper) => mapper.Map(It.IsAny<IEnumerable<EstablishmentDataTransferObject>>()),
                Times.Once);

        Assert.NotNull(result);
        Assert.Equal(_mappedResponseDtos, result);
    }
}
