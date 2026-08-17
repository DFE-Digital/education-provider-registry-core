using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class FilterMapperTestDouble
{
    public static Mock<IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>>> Mock(
        ReadOnlyCollection<SearchFilterRequest>? mapped = null)
    {
        Mock<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>> mock = new(MockBehavior.Strict);

        mock.Setup(mapper =>
            mapper.Map(It.IsAny<ReadOnlyCollection<FilterRequest>>()))
            .Returns(mapped ??
                new ReadOnlyCollection<SearchFilterRequest>(
                    Array.Empty<SearchFilterRequest>()));

        return mock;
    }
}
