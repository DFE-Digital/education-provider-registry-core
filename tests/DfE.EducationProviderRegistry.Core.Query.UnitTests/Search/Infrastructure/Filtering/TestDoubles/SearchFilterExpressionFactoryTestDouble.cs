using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterExpressionFactoryTestDouble
{
    public static Mock<ISearchFilterExpressionFactory> Mock() =>
        new(MockBehavior.Strict);

    public static (Mock<ISearchFilterExpressionFactory> factory, Mock<ISearchFilterExpression> expression)
        MockFor(string filterKey, string expressionValue)
    {
        Mock<ISearchFilterExpression> exprMock = new(MockBehavior.Strict);
        exprMock
            .Setup(searchFilterExpression =>
                searchFilterExpression.GetFilterExpression(It.IsAny<SearchFilterRequest>()))
            .Returns(expressionValue)
            .Verifiable();

        Mock<ISearchFilterExpressionFactory> factoryMock = Mock();

        factoryMock
            .Setup(searchFilterExpressionFactory =>
                searchFilterExpressionFactory.CreateFilter(filterKey))
            .Returns(exprMock.Object)
            .Verifiable();

        return (factoryMock, exprMock);
    }
}
