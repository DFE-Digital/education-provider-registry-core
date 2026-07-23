using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using Microsoft.Extensions.Options;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering;

public sealed class SearchFilterExpressionsBuilderUnitTests
{
    private static FilterKeyToFilterExpressionMapOptions Options(
        string logicalOperator,
        Dictionary<string, FilterExpressionOptions> map)
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            FilterChainingLogicalOperator = logicalOperator,
            SearchFilterToExpressionMap = map
        };
    }

    private static SearchFilterExpressionsBuilder<DummyProjection> Builder(
        FilterKeyToFilterExpressionMapOptions options,
        ISearchFilterExpressionFactory<DummyProjection> exprFactory)
    {
        IOptions<FilterKeyToFilterExpressionMapOptions> wrapped =
            Microsoft.Extensions.Options.Options.Create(options);

        return new SearchFilterExpressionsBuilder<DummyProjection>(
            exprFactory,
            wrapped);
    }

    private static SearchFilterRequest[] Requests(
        params SearchFilterRequest[] requests) => requests;

    private static Expression<Func<DummyProjection, bool>> Lambda(
        Expression body,
        ParameterExpression param) =>
            Expression.Lambda<Func<DummyProjection, bool>>(body, param);

    [Fact]
    public void BuildSearchFilterExpression_NoMatchingKeys_ReturnsTrueExpression()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options("AND", []);

        Mock<ISearchFilterExpressionFactory<DummyProjection>> exprFactoryMock =
            SearchFilterExpressionFactoryTestDouble.Mock<DummyProjection>();

        Mock<ILogicalOperatorFactory<DummyProjection>> opFactoryMock =
            LogicalOperatorFactoryTestDouble.Mock<DummyProjection>();

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        SearchFilterRequest[] requests =
            Requests(new SearchFilterRequest("UNKNOWN", [ "A" ]));

        // act
        Expression<Func<DummyProjection, bool>> expr =
            builder.BuildSearchFilterExpression(requests);

        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "anything" }));

        exprFactoryMock.Verify(
            searchFilterExpressionFactory =>
                searchFilterExpressionFactory.ComposeFilters(
                It.IsAny<IReadOnlyList<(string, SearchFilterRequest)>>(),
                It.IsAny<string>()),
            Times.Never());

        opFactoryMock.Verify(
            logicalOperatorFactory =>
                logicalOperatorFactory.Resolve(It.IsAny<string>()),
            Times.Never());
    }

    [Fact]
    public void BuildSearchFilterExpression_SingleFilter_ResolvesCorrectExpression()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options("AND", new Dictionary<string, FilterExpressionOptions>
            {
                { "eq", new FilterExpressionOptions { FilterExpressionKey = "Equals" } }
            });

        Expression<Func<DummyProjection, bool>> expectedExpr =
            projection => projection.Value == "A";

        Mock<ISearchFilterExpressionFactory<DummyProjection>> exprFactoryMock =
            SearchFilterExpressionFactoryTestDouble
                .MockComposition("AND", expectedExpr);

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        SearchFilterRequest[] requests =
            Requests(new SearchFilterRequest("eq", [ "A" ]));

        // act
        Expression<Func<DummyProjection, bool>> expr =
            builder.BuildSearchFilterExpression(requests);

        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
    }

    [Fact]
    public void BuildSearchFilterExpression_MultipleFilters_ComposesCorrectly()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options("AND", new Dictionary<string, FilterExpressionOptions>
            {
                { "eq", new FilterExpressionOptions { FilterExpressionKey = "Equals" } },
                { "neq", new FilterExpressionOptions { FilterExpressionKey = "NotEquals" } }
            });

        ParameterExpression param = Expression.Parameter(typeof(DummyProjection), "param");

        Expression<Func<DummyProjection, bool>> exprA =
            Lambda(
                Expression.Equal(
                    Expression.Property(param, nameof(DummyProjection.Value)),
                    Expression.Constant("A")),
                param);

        Expression<Func<DummyProjection, bool>> exprB =
            Lambda(
                Expression.NotEqual(
                    Expression.Property(param, nameof(DummyProjection.Value)),
                    Expression.Constant("B")),
                param);

        Mock<ISearchFilterExpression<DummyProjection>> eqMock =
            SearchFilterExpressionTestDouble.MockForExpression(exprA);

        Mock<ISearchFilterExpression<DummyProjection>> neqMock =
            SearchFilterExpressionTestDouble.MockForExpression(exprB);

        Expression<Func<DummyProjection, bool>> composed =
            Lambda(Expression.AndAlso(exprA.Body, exprB.Body), param);

        Mock<ISearchFilterExpressionFactory<DummyProjection>> exprFactoryMock =
            SearchFilterExpressionFactoryTestDouble.MockComposition(
                "AND",
                composed);

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        SearchFilterRequest[] requests =
            Requests(
                new SearchFilterRequest("eq", [ "A" ]),
                new SearchFilterRequest("neq", [ "B" ]));

        // act
        Expression<Func<DummyProjection, bool>> expr =
            builder.BuildSearchFilterExpression(requests);

        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
        Assert.False(compiled(new DummyProjection { Value = "B" }));
    }

    [Fact]
    public void BuildSearchFilterExpression_MissingLogicalOperator_ReturnsTrueExpression()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options(null!, []);

        Mock<ISearchFilterExpressionFactory<DummyProjection>> exprFactoryMock =
            SearchFilterExpressionFactoryTestDouble.Mock<DummyProjection>();

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        // act
        Expression<Func<DummyProjection, bool>> expr =
            builder.BuildSearchFilterExpression(Array.Empty<SearchFilterRequest>());

        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "anything" }));
    }
}
