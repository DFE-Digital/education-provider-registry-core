using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class FilterResultsMapperTestDouble
{
    public static Mock<IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>>> MockFor(
            ReadOnlyCollection<SearchFilterRequest> searchFilterRequests)
    {
        Mock<IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>>> filtersMapper = Mock();

        filtersMapper
            .Setup(mapper =>
                mapper.Map(It.IsAny<ReadOnlyCollection<FilterRequest>>()))
        .Returns(searchFilterRequests)
        .Verifiable();

        return filtersMapper;
    }
}
