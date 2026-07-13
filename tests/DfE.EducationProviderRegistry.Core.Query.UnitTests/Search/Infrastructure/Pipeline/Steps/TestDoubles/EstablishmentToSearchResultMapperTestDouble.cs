using System.Diagnostics.CodeAnalysis;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentToSearchResultMapperTestDouble
{
    public static Mock<IMapper<Establishment, EstablishmentSearchResult>> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<IMapper<Establishment, EstablishmentSearchResult>>
        MockFor(Func<Establishment, EstablishmentSearchResult> establishmentSearchResultDelegate)
    {
        Mock<IMapper<Establishment, EstablishmentSearchResult>> mapperMock = Mock();

        mapperMock
            .Setup(mapper =>
                mapper.Map(It.IsAny<Establishment>()))
                    .Returns(establishmentSearchResultDelegate);

        return mapperMock;
    }

    public static Mock<IMapper<Establishment, EstablishmentSearchResult>>
        MockFor(EstablishmentSearchResult establishmentSearchResult)
    {
        Mock<IMapper<Establishment, EstablishmentSearchResult>> mapperMock = Mock();

        mapperMock
            .Setup(mapper =>
                mapper.Map(It.IsAny<Establishment>()))
                    .Returns(establishmentSearchResult);

        return mapperMock;
    }
}
