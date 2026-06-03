using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers.TestDoubles;

public static class EstablishmentsCollectionMapperTestDouble
{
    public static Mock<
        IMapper<IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>>> Default() => new();

    public static Mock<
        IMapper<IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>>> MockMapReturns(
            IEnumerable<EstablishmentDataTransferObject> input,
            IReadOnlyCollection<Establishment> output)
    {
        Mock<IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>> mock =
            Default();

        mock.Setup(m => m.Map(input))
            .Returns(output)
            .Verifiable();

        return mock;
    }

    public static Mock<
        IMapper<IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>>> MockMapThrows(
            Exception exception)
    {
        Mock<IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>> mock =
            Default();

        mock.Setup(m => m.Map(It.IsAny<IEnumerable<EstablishmentDataTransferObject>>()))
            .Throws(exception)
            .Verifiable();

        return mock;
    }

    public static Mock<
        IMapper<IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>>> MockVerifyCalled()
    {
        Mock<IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>> mock =
            Default();

        mock.Setup(m => m.Map(It.IsAny<IEnumerable<EstablishmentDataTransferObject>>()))
            .Verifiable();

        return mock;
    }
}
