using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class LogicalOperatorFactoryTestDouble
{
    public static Mock<ILogicalOperatorFactory> Mock() =>
        new(MockBehavior.Strict);

    public static (Mock<ILogicalOperatorFactory> factory, Mock<ILogicalOperator> op)
    MockFor(string opKey, string opExpression)
    {
        Mock<ILogicalOperator> opMock = new(MockBehavior.Strict);
        opMock.Setup(logicalOperator =>
            logicalOperator.GetOperatorExpression()).Returns(opExpression);

        Mock<ILogicalOperatorFactory> factoryMock = new(MockBehavior.Strict);
        factoryMock.Setup(logicalOperatorFactory =>
            logicalOperatorFactory.CreateLogicalOperator(opKey))
                   .Returns(opMock.Object);

        return (factoryMock, opMock);
    }

}
