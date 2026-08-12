using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using Microsoft.Extensions.Options;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering;

public sealed class SearchFilterExpressionsBuilderUnitTests
{
    private static FilterKeyToFilterExpressionMapOptions Options(
        Dictionary<string, FilterExpressionOptions> map)
    {
        return new FilterKeyToFilterExpressionMapOptions
        {
            SearchFilterToExpressionMap = map
        };
    }

    private static SearchFilterExpressionsBuilder<DummyProjection> Builder(
        FilterKeyToFilterExpressionMapOptions options,
        ISearchFilterSpecificationFactory<DummyProjection> exprFactory)
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
    public void BuildSearchFilterExpression_UnknownFilterKey_ThrowsInvalidOperationException()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options([]);

        Mock<ISearchFilterSpecificationFactory<DummyProjection>> exprFactoryMock =
            SearchFilterFactoryTestDouble.Mock<DummyProjection>();

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        SearchFilterRequest[] requests =
            Requests(new SearchFilterRequest("UNKNOWN", ["A"]));

        // act
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => builder.BuildSearchFilterExpression(requests));

        // assert
        Assert.Equal(
            "No filter expression configuration exists for 'UNKNOWN'.",
            exception.Message);

        exprFactoryMock.Verify(
            searchFilterSpecificationFactory =>
                searchFilterSpecificationFactory.Create(
                    It.IsAny<string>(),
                    It.IsAny<SearchFilterRequest>()),
            Times.Never());
    }

    [Fact]
    public void BuildSearchFilterExpression_NoRequests_ReturnsTrueExpression()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options([]);

        Mock<ISearchFilterSpecificationFactory<DummyProjection>> exprFactoryMock =
            SearchFilterFactoryTestDouble.Mock<DummyProjection>();

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        SearchFilterRequest[] requests = [];

        // act
        Expression<Func<DummyProjection, bool>> expression =
            builder.BuildSearchFilterExpression(requests);

        Func<DummyProjection, bool> compiled = expression.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "anything" }));

        exprFactoryMock.Verify(
            searchFilterSpecificationFactory =>
                searchFilterSpecificationFactory.Create(
                    It.IsAny<string>(),
                    It.IsAny<SearchFilterRequest>()),
            Times.Never());
    }

    [Fact]
    public void BuildSearchFilterExpression_SingleFilter_ResolvesCorrectExpression()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options(new Dictionary<string, FilterExpressionOptions>
            {
            { "eq", new FilterExpressionOptions { FilterExpressionKey = "Equals" } }
            });

        Expression<Func<DummyProjection, bool>> expectedExpr =
            projection => projection.Value == "A";

        ISpecification<DummyProjection> specification =
            SpecificationTestDoubles.Create(expectedExpr);

        Mock<ISearchFilterSpecificationFactory<DummyProjection>> exprFactoryMock =
            new();

        exprFactoryMock
            .Setup(x => x.Create(It.IsAny<string>(), It.IsAny<SearchFilterRequest>()))
            .Returns(specification);

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        SearchFilterRequest[] requests =
            Requests(new SearchFilterRequest("eq", ["A"]));

        // act
        Expression<Func<DummyProjection, bool>> expr =
            builder.BuildSearchFilterExpression(requests);

        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
        Assert.False(compiled(new DummyProjection { Value = "B" }));

        exprFactoryMock.Verify(
            x => x.Create("Equals", It.IsAny<SearchFilterRequest>()),
            Times.Once);
    }

    [Fact]
    public void BuildSearchFilterExpression_MultipleFilters_ComposesCorrectly()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options(new Dictionary<string, FilterExpressionOptions>
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

        ISpecification<DummyProjection> specA =
            SpecificationTestDoubles.Create(exprA);

        ISpecification<DummyProjection> specB =
            SpecificationTestDoubles.Create(exprB);

        Mock<ISearchFilterSpecificationFactory<DummyProjection>> exprFactoryMock =
            new();

        exprFactoryMock
            .Setup(x => x.Create(It.Is<string>(t => t == "Equals"), It.IsAny<SearchFilterRequest>()))
            .Returns(specA);

        exprFactoryMock
            .Setup(x => x.Create(It.Is<string>(t => t == "NotEquals"), It.IsAny<SearchFilterRequest>()))
            .Returns(specB);

        SearchFilterExpressionsBuilder<DummyProjection> builder =
            Builder(options, exprFactoryMock.Object);

        SearchFilterRequest[] requests =
            Requests(
                new SearchFilterRequest("eq", ["A"]),
                new SearchFilterRequest("neq", ["B"]));

        // act
        Expression<Func<DummyProjection, bool>> expr =
            builder.BuildSearchFilterExpression(requests);

        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
        Assert.False(compiled(new DummyProjection { Value = "B" }));
        exprFactoryMock.Verify(
            x => x.Create("Equals", It.IsAny<SearchFilterRequest>()),
            Times.Once);

        exprFactoryMock.Verify(
            x => x.Create("NotEquals", It.IsAny<SearchFilterRequest>()),
            Times.Once);
    }

    [Fact]
    public void BuildSearchFilterExpression_EmptyConfiguration_ReturnsTrueExpression()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            Options([]);

        Mock<ISearchFilterSpecificationFactory<DummyProjection>> exprFactoryMock =
            SearchFilterFactoryTestDouble.Mock<DummyProjection>();

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
