using System;
using System.Collections.Generic;
using System.Text;
using DfE.Core.Libraries.CrossCutting.Mapper;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;

internal static class IMapperTestDouble
{
    internal static Mock<IMapper<TIn, TOut>> For<TIn, TOut>(TOut output)
    {
        return MockTestDouble.For<IMapper<TIn, TOut>, TOut>(
            (mapper) => mapper.Map(It.IsAny<TIn>()),
                output);
    }
}
