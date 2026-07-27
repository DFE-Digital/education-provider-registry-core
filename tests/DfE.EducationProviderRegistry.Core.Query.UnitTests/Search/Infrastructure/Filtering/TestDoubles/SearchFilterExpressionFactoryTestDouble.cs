using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterExpressionFactoryTestDouble
{
    public static Mock<ISearchFilterExpressionFactory<TProjection>> Mock<TProjection>()
        where TProjection : class
    {
        return new Mock<ISearchFilterExpressionFactory<TProjection>>(MockBehavior.Strict);
    }

    public static (
        Mock<ISearchFilterExpressionFactory<TProjection>> factory,
        Mock<ISearchFilter<TProjection>> expression
    ) MockFor<TProjection>(
        string filterKey,
        Expression<Func<TProjection, bool>> expressionTree)
        where TProjection : class
    {
        // Mock the filter expression itself
        Mock<ISearchFilter<TProjection>> exprMock =
            SearchFilterExpressionTestDouble.MockForExpression<TProjection>(expressionTree);

        exprMock
            .Setup(searchFilterExpression =>
                searchFilterExpression.ToExpression(It.IsAny<SearchFilterRequest>()))
            .Returns(expressionTree)
            .Verifiable();

        Mock<ISearchFilterExpressionFactory<TProjection>> factoryMock = Mock<TProjection>();

        factoryMock
            .Setup(f => f.ComposeFilters(
                It.Is<IReadOnlyList<(string FilterName, SearchFilterRequest Request)>>(
                    list => list.Any(item => item.FilterName == filterKey)),
                It.IsAny<string>()))
            .Returns(expressionTree)
            .Verifiable();

        return (factoryMock, exprMock);
    }

    public static Mock<ISearchFilterExpressionFactory<TProjection>> MockComposition<TProjection>(
        string logicalOperatorName,
        Expression<Func<TProjection, bool>> expression)
        where TProjection : class
    {

        Mock<ISearchFilterExpressionFactory<TProjection>> factoryMock = Mock<TProjection>();

        factoryMock
            .Setup(searchFilterExpressionFactory =>
                searchFilterExpressionFactory.ComposeFilters(
                It.IsAny<IReadOnlyList<(string FilterName, SearchFilterRequest Request)>>(),
                logicalOperatorName))
            .Returns(expression);

        return factoryMock;
    }

    public static void MockComposition<TProjection>(
        string logicalOperatorName,
        Func<IReadOnlyList<(string FilterName, SearchFilterRequest Request)>,
             string,
             Expression<Func<TProjection, bool>>> composer)
    where TProjection : class
    {
        Mock<ISearchFilterExpressionFactory<TProjection>> factoryMock = Mock<TProjection>();

        factoryMock
            .Setup(searchFilterExpressionFactory =>
                searchFilterExpressionFactory.ComposeFilters(
                    It.IsAny<IReadOnlyList<(string FilterName, SearchFilterRequest Request)>>(),
                    logicalOperatorName))
            .Returns((IReadOnlyList<(string FilterName, SearchFilterRequest Request)> filters,
                      string op) =>
            {
                return composer(filters, op);
            });
    }
}
