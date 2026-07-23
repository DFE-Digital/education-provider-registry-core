using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class FilterExpressionFactoryUnitTests
{
    private FilterExpressionFactory<DummyProjection> CreateFactory()
    {
        Mock<ISearchFilterExpression<DummyProjection>> filterExprA =
            SearchFilterExpressionTestDouble.MockEquals("A");

        Mock<ISearchFilterExpression<DummyProjection>> filterExprB =
            SearchFilterExpressionTestDouble.MockNotEquals("B");

        Dictionary<string, Func<ISearchFilterExpression<DummyProjection>>> registry =
            new()
            {
                { "Equals", () => filterExprA.Object },
                { "NotEquals", () => filterExprB.Object }
            };

        ILogicalOperatorFactory<DummyProjection> logicalOperatorFactory =
            LogicalOperatorFactoryTestDouble.MockFactoryWithRegistry<DummyProjection>(
                andOperator: LogicalOperatorTestDoubles.MockAnd<DummyProjection>(),
                orOperator: LogicalOperatorTestDoubles.MockOr<DummyProjection>()).Object;

        return new FilterExpressionFactory<DummyProjection>(registry, logicalOperatorFactory);
    }

    [Fact]
    public void CreateFilter_ReturnsCorrectExpression()
    {
        // arrange
        FilterExpressionFactory<DummyProjection> factory = CreateFactory();
        SearchFilterRequest request = new("Value", ["A"]);

        Expression<Func<DummyProjection, bool>> expr =
            factory.CreateFilter("Equals", request);

        // act
        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
        Assert.False(compiled(new DummyProjection { Value = "B" }));
    }

    [Fact]
    public void CreateFilter_UnknownName_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        FilterExpressionFactory<DummyProjection> factory = CreateFactory();
        SearchFilterRequest request = new("Value", ["A"]);

        // assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            factory.CreateFilter("DoesNotExist", request));
    }

    [Fact]
    public void ComposeFilters_NoFilters_ReturnsTrueExpression()
    {
        // arrange
        FilterExpressionFactory<DummyProjection> factory = CreateFactory();

        Expression<Func<DummyProjection, bool>> expr =
            factory.ComposeFilters(Array.Empty<(string, SearchFilterRequest)>(), "AND");

        // act
        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "Anything" }));
    }

    [Fact]
    public void ComposeFilters_AND_ComposesCorrectly()
    {
        // arrange
        Mock<ISearchFilterExpression<DummyProjection>> filterExprA =
            SearchFilterExpressionTestDouble.MockEquals("A");
        Mock<ISearchFilterExpression<DummyProjection>> filterExprB =
            SearchFilterExpressionTestDouble.MockNotEquals("B");

        ILogicalOperator<DummyProjection> andOperator =
            LogicalOperatorTestDoubles.MockAnd<DummyProjection>().Object;

        Mock<ILogicalOperatorFactory<DummyProjection>> logicalOperatorFactory =
            LogicalOperatorFactoryTestDouble.MockFactoryWithRegistry<DummyProjection>(
                andOperator: LogicalOperatorTestDoubles.MockAnd<DummyProjection>(),
                orOperator: LogicalOperatorTestDoubles.MockOr<DummyProjection>());

        FilterExpressionFactory<DummyProjection> factory =
            new(
                new Dictionary<string, Func<ISearchFilterExpression<DummyProjection>>>
                {
                    { "A", () => filterExprA.Object },
                    { "B", () => filterExprB.Object }
                },
                logicalOperatorFactory.Object);

        (string, SearchFilterRequest)[] filters =
        [
            ("A", new SearchFilterRequest("Value", ["A"])),
            ("B", new SearchFilterRequest("Value", ["B"]))
        ];

        Expression<Func<DummyProjection, bool>> combined =
            factory.ComposeFilters(filters, "AND");

        // act
        Func<DummyProjection, bool> compiled = combined.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
        Assert.False(compiled(new DummyProjection { Value = "B" }));
    }

    [Fact]
    public void ComposeFilters_OR_ComposesCorrectly()
    {
        // arrange
        Mock<ISearchFilterExpression<DummyProjection>> filterExprA =
            SearchFilterExpressionTestDouble.MockEquals("A");
        Mock<ISearchFilterExpression<DummyProjection>> filterExprB =
            SearchFilterExpressionTestDouble.MockEquals("B");

        ILogicalOperator<DummyProjection> orOperator =
            LogicalOperatorTestDoubles.MockOr<DummyProjection>().Object;

        Mock<ILogicalOperatorFactory<DummyProjection>> logicalOperatorFactory =
            LogicalOperatorFactoryTestDouble.MockFactoryWithRegistry<DummyProjection>(
                andOperator: LogicalOperatorTestDoubles.MockAnd<DummyProjection>(),
                orOperator: LogicalOperatorTestDoubles.MockOr<DummyProjection>());

        FilterExpressionFactory<DummyProjection> factory =
            new(
                new Dictionary<string, Func<ISearchFilterExpression<DummyProjection>>>
                {
                    { "A", () => filterExprA.Object },
                    { "B", () => filterExprB.Object }
                },
                logicalOperatorFactory.Object);

        (string, SearchFilterRequest)[] filters =
        {
            ("A", new SearchFilterRequest("Value", ["A"])),
            ("B", new SearchFilterRequest("Value", ["B"]))
        };

        Expression<Func<DummyProjection, bool>> combined =
            factory.ComposeFilters(filters, "OR");

        // act
        Func<DummyProjection, bool> compiled = combined.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
        Assert.True(compiled(new DummyProjection { Value = "B" }));
        Assert.False(compiled(new DummyProjection { Value = "C" }));
    }

    [Fact]
    public void ComposeFilters_WithInstances_ComposesCorrectly()
    {
        // arrange
        FilterExpressionFactory<DummyProjection> factory = CreateFactory();

        (ISearchFilterExpression<DummyProjection> Filter, SearchFilterRequest Request)[] filters =
        [
            (SearchFilterExpressionTestDouble.MockEquals("A").Object, new SearchFilterRequest("Value", ["A"])),
            (SearchFilterExpressionTestDouble.MockNotEquals("B").Object, new SearchFilterRequest("Value", ["B"]))
        ];

        Expression<Func<DummyProjection, bool>> expr =
            factory.ComposeFilters(filters, "AND");

        // act
        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { Value = "A" }));
        Assert.False(compiled(new DummyProjection { Value = "B" }));
    }
}
