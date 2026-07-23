using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class LogicalOperatorFactoryTestDouble
{
    public static Mock<ILogicalOperatorFactory<TProjection>> Mock<TProjection>()
        where TProjection : class =>
            new(MockBehavior.Strict);

    public static Mock<ILogicalOperatorFactory<TProjection>> MockFor<TProjection>(
        string opKey,
        Mock<ILogicalOperator<TProjection>> operatorMock)
        where TProjection : class
    {
        Mock<ILogicalOperatorFactory<TProjection>> factoryMock = Mock<TProjection>();

        factoryMock
            .Setup(logicalOperatorFactory =>
                logicalOperatorFactory.Resolve(opKey))
            .Returns(operatorMock.Object);

        return factoryMock;
    }

    public static Mock<ILogicalOperatorFactory<TProjection>> MockFactoryWithRegistry<TProjection>(
        Mock<ILogicalOperator<TProjection>> andOperator,
        Mock<ILogicalOperator<TProjection>> orOperator)
        where TProjection : class
    {
        Mock<ILogicalOperatorFactory<TProjection>> factoryMock = Mock<TProjection>();

        factoryMock
            .Setup(logicalOperatorFactory =>
                logicalOperatorFactory.Resolve("AND"))
            .Returns(andOperator.Object);

        factoryMock
            .Setup(logicalOperatorFactory =>
                logicalOperatorFactory.Resolve("OR"))
            .Returns(orOperator.Object);

        return factoryMock;
    }
}
