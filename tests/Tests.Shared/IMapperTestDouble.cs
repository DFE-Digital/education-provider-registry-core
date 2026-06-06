using DfE.Core.Libraries.CrossCutting.Mapper;
using Moq;

namespace Tests.Shared;

public static class IMapperTestDouble
{
    public static Mock<IMapper<TIn, TOut>> For<TIn, TOut>(TOut output)
    {
        return MockTestDouble.For<IMapper<TIn, TOut>, TOut>(
            (mapper) => mapper.Map(It.IsAny<TIn>()),
                output);
    }
}
