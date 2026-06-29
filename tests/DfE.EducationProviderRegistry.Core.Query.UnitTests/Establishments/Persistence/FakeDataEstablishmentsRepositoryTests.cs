using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence;

public sealed class FakeDataEstablishmentsRepositoryTests
{
    private readonly Mock<IMapper<Establishment, EstablishmentDetailsModel>> _singleMapper;
    private readonly Mock<IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentDetailsModel>>> _collectionMapper;
    private readonly IReadOnlyCollection<EstablishmentDetailsModel> _mappedResponseDtos;
    private readonly FakeDataEstablishmentsRepository _sut;

    public FakeDataEstablishmentsRepositoryTests()
    {
        _mappedResponseDtos =
            new EstablishmentCollectionBuilder()
                .WithCount(100)
                .Build();

        _singleMapper =
            IMapperTestDouble.Map<Establishment, EstablishmentDetailsModel>(
                output: _mappedResponseDtos.First());

        _collectionMapper =
            IMapperTestDouble.Map<
                IEnumerable<Establishment>,
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
            (mapper) => mapper.Map(It.IsAny<IEnumerable<Establishment>>()),
                Times.Once);

        Assert.NotNull(result);
        Assert.Equal(_mappedResponseDtos, result);
    }
}
