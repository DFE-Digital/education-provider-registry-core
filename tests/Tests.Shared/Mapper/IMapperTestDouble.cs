using DfE.Core.Libraries.CrossCutting.Mapper;
using Moq;

namespace Tests.Shared.Mapper;

public static class IMapperTestDouble
{
    public static Mock<IMapper<TIn, TOut>> Map<TIn, TOut>(TOut output)
    {
        return MockTestDouble.For<IMapper<TIn, TOut>, TOut>(
            (mapper) => mapper.Map(It.IsAny<TIn>()),
                output);
    }

    public static Mock<IMapper<TIn, TOut>> MapMany<TIn, TOut>(IEnumerable<KeyValuePair<TIn, TOut>> outputByInput)
    {
        Mock<IMapper<TIn, TOut>> mock = MockTestDouble.Default<IMapper<TIn, TOut>>();

        foreach ((TIn input, TOut output) in outputByInput)
        {
            mock.Setup(m => m.Map(input))
                .Returns(output)
                .Verifiable();
        }

        return mock;
    }
}
