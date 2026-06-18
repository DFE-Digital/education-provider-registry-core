using DfE.Core.Libraries.CrossCutting.Mapper;
using Moq;

namespace Tests.Shared.Mapper;

public static class MapperVerifyExtensions
{
    public static void VerifyMapperCalled<TSource, TDestination>(
        this Mock<IMapper<TSource, TDestination>> mock,
        int count = 1)
    {
        mock.Verify(
            mapper => mapper.Map(It.IsAny<TSource>()),
            Times.Exactly(count));
    }

    public static void VerifyMapperCalledWith<TSource, TDestination>(
        this Mock<IMapper<TSource, TDestination>> mock,
        TSource expectedInput,
        int count = 1)
    {
        mock.Verify(
            mapper => mapper.Map(expectedInput),
            Times.Exactly(count));
    }
}
