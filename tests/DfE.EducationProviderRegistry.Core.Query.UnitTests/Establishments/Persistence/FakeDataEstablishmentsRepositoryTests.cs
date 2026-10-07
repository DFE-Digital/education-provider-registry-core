using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence;

public sealed class FakeDataEstablishmentsRepositoryTests
{
    private readonly Mock<IMapper<EstablishmentReadModel, EstablishmentDetailsModel>> _singleMapper;
    private readonly Mock<IMapper<IEnumerable<EstablishmentReadModel>, IReadOnlyCollection<EstablishmentDetailsModel>>> _collectionMapper;
    private readonly IReadOnlyCollection<EstablishmentDetailsModel> _mappedResponseDtos;
    private readonly FakeDataEstablishmentsRepository _sut;

    public FakeDataEstablishmentsRepositoryTests()
    {
        _mappedResponseDtos =
            new EstablishmentDetailsModelCollectionBuilder()
                .WithCount(100)
                .Build();

        _singleMapper =
            IMapperTestDouble.Map<EstablishmentReadModel, EstablishmentDetailsModel>(
                output: _mappedResponseDtos.First());

        _collectionMapper =
            IMapperTestDouble.Map<
                IEnumerable<EstablishmentReadModel>,
                IReadOnlyCollection<EstablishmentDetailsModel>>(
                    output: _mappedResponseDtos);

        _sut = new FakeDataEstablishmentsRepository(
            establishmentMapper: _singleMapper.Object,
            establishmentsMapper: _collectionMapper.Object);
    }

    [Fact]
    public async Task GetEstablishments_ShouldMapDtosGeneratedByBuilder()
    {
        // Arrange Act
        IReadOnlyCollection<EstablishmentDetailsModel> result =
            await _sut.GetEstablishments(CancellationToken.None);

        // Assert
        _collectionMapper.Verify(
            (mapper) => mapper.Map(It.IsAny<IEnumerable<EstablishmentReadModel>>()),
                Times.Once);

        Assert.NotNull(result);
        Assert.Equal(_mappedResponseDtos, result);
    }
}
