using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers.TestDoubles.StubBuilders;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence;

public sealed class EstablishmentsRepositoryTests
{
    private readonly EstablishmentsRepository _sut;

    public EstablishmentsRepositoryTests()
    {
        // Real single-item mapper
        IMapper<EstablishmentDataTransferObject, Establishment> singleMapper =
            new EstablishmentDtoToModelMapper();

        // Real collection mapper
        IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>> collectionMapper =
            new EstablishmentsDtoToModelMapper(singleMapper);

        _sut = new EstablishmentsRepository(collectionMapper);
    }

    [Fact]
    public async Task GetEstablishments_ShouldMapDtosGeneratedByBuilder()
    {
        // Arrange
        EstablishmentDataTransferObjectBuilder builder = new();
        IReadOnlyCollection<EstablishmentDataTransferObject> dtos = builder.BuildMany(100);

        // Act
        IReadOnlyCollection<Establishment> result =
            await _sut.GetEstablishments(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.Count);

        foreach (Establishment establishment in result)
        {
            string urn = establishment.Identifier.Urn;

            Assert.NotNull(urn);
            Assert.Equal(6, urn.Length);
            Assert.True(urn.All(char.IsDigit));
        }
    }

    [Fact]
    public async Task GetEstablishments_ShouldRespectCancellationToken()
    {
        // Arrange
        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

        // Act
        IReadOnlyCollection<Establishment> result =
            await _sut.GetEstablishments(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.Count);
    }
}
