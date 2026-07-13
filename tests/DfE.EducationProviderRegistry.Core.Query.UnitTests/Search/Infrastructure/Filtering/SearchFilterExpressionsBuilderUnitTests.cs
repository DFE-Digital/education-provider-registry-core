using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Options.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using Microsoft.Extensions.Options;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering;

public sealed class SearchFilterExpressionsBuilderUnitTests
{
    private static SearchFilterExpressionsBuilder CreateBuilder(
        IOptions<FilterKeyToFilterExpressionMapOptions> options,
        Mock<ISearchFilterExpressionFactory> exprFactoryMock,
        Mock<ILogicalOperatorFactory> opFactoryMock)
    {
        return new SearchFilterExpressionsBuilder(
            exprFactoryMock.Object,
            opFactoryMock.Object,
            options);
    }

    [Fact]
    public void BuildSearchFilterExpressions_WhenNoMatchingKeys_ReturnsEmptyString()
    {
        // arrange
        IOptions<FilterKeyToFilterExpressionMapOptions> options =
            FilterKeyToFilterExpressionMapOptionsStub.EmptyMapIOptions();

        Mock<ISearchFilterExpressionFactory> exprFactoryMock =
            SearchFilterExpressionFactoryTestDouble.Mock();

        (Mock<ILogicalOperatorFactory> opFactoryMock,
         Mock<ILogicalOperator> _) =
            LogicalOperatorFactoryTestDouble.MockFor("AND", " AND ");

        SearchFilterExpressionsBuilder builder =
            CreateBuilder(options, exprFactoryMock, opFactoryMock);

        SearchFilterRequest[] requests =
        [
            SearchFilterRequestStub.Create("UNKNOWN", "A")
        ];

        // act
        string result = builder.BuildSearchFilterExpressions(requests);

        // assert/verify
        Assert.Equal(string.Empty, result);
        exprFactoryMock.Verify(searchFilterExpressionFactory =>
            searchFilterExpressionFactory.CreateFilter(It.IsAny<string>()), Times.Never());
        opFactoryMock.Verify(logicalOperatorFactory =>
            logicalOperatorFactory.CreateLogicalOperator("AND"), Times.Once());
    }

    [Fact]
    public void BuildSearchFilterExpressions_WhenSingleFilter_ReturnsExpression_AndVerifiesFactoryCalls()
    {
        // arrange
        IOptions<FilterKeyToFilterExpressionMapOptions> options =
            FilterKeyToFilterExpressionMapOptionsStub.CreateIOptions(
            FilterKeyToFilterExpressionMapOptionsStub.Create(
                "AND",
                new Dictionary<string, FilterExpressionOptions>
                {
                    { "CODE", new FilterExpressionOptions { FilterExpressionKey = "SearchIn" } }
                }));

        (Mock<ISearchFilterExpressionFactory> exprFactoryMock,
         Mock<ISearchFilterExpression> exprMock) =
            SearchFilterExpressionFactoryTestDouble.MockFor("SearchIn", "expr");

        (Mock<ILogicalOperatorFactory> opFactoryMock,
         Mock<ILogicalOperator> opMock) =
            LogicalOperatorFactoryTestDouble.MockFor("AND", " AND ");

        SearchFilterExpressionsBuilder builder =
            CreateBuilder(options, exprFactoryMock, opFactoryMock);

        SearchFilterRequest[] requests =
        [
            SearchFilterRequestStub.Create("CODE", "A")
        ];

        // act
        string result = builder.BuildSearchFilterExpressions(requests);

        // assert/verify
        Assert.Equal("expr", result);
        exprFactoryMock.Verify(searchFilterExpressionFactory =>
            searchFilterExpressionFactory.CreateFilter("SearchIn"), Times.Once());
        exprMock.Verify(searchFilterExpression =>
            searchFilterExpression.GetFilterExpression(It.IsAny<SearchFilterRequest>()), Times.Once());
        opFactoryMock.Verify(logicalOperatorFactory =>
            logicalOperatorFactory.CreateLogicalOperator("AND"), Times.Once());
        opMock.Verify(logicalOperator => logicalOperator.GetOperatorExpression(), Times.Once());
    }

    [Fact]
    public void BuildSearchFilterExpressions_WhenMultipleFilters_AppliesLogicalOperator_AndVerifiesCalls()
    {
        // arrange
        IOptions<FilterKeyToFilterExpressionMapOptions> options =
            FilterKeyToFilterExpressionMapOptionsStub.CreateIOptions(
            FilterKeyToFilterExpressionMapOptionsStub.Create(
                "AND",
                new Dictionary<string, FilterExpressionOptions>
                {
                    { "A", new FilterExpressionOptions { FilterExpressionKey = "SearchIn" } },
                    { "B", new FilterExpressionOptions { FilterExpressionKey = "SearchIn" } }
                }));

        (Mock<ISearchFilterExpressionFactory> exprFactoryMock,
         Mock<ISearchFilterExpression> exprMock) =
            SearchFilterExpressionFactoryTestDouble.MockFor("SearchIn", "X");

        (Mock<ILogicalOperatorFactory> opFactoryMock,
         Mock<ILogicalOperator> opMock) =
            LogicalOperatorFactoryTestDouble.MockFor("AND", " AND ");

        SearchFilterExpressionsBuilder builder =
            CreateBuilder(options, exprFactoryMock, opFactoryMock);

        SearchFilterRequest[] requests =
        [
            SearchFilterRequestStub.Create("A", "1"),
            SearchFilterRequestStub.Create("B", "2")
        ];

        // act
        string result = builder.BuildSearchFilterExpressions(requests);

        // assert/verify
        Assert.Equal("X AND X", result);

        exprFactoryMock.Verify(searchFilterExpressionFactory =>
            searchFilterExpressionFactory.CreateFilter("SearchIn"), Times.Exactly(2));
        exprMock.Verify(searchFilterExpression =>
            searchFilterExpression.GetFilterExpression(It.IsAny<SearchFilterRequest>()), Times.Exactly(2));

        opFactoryMock.Verify(logicalOperatorFactory =>
            logicalOperatorFactory.CreateLogicalOperator("AND"), Times.Once());
        opMock.Verify(logicalOperator =>
            logicalOperator.GetOperatorExpression(), Times.Once());
    }

    [Fact]
    public void GetValidSearchFilterExpression_AssignsDelimiter_AndVerifiesCalls()
    {
        // arrange
        IOptions<FilterKeyToFilterExpressionMapOptions> options =
            FilterKeyToFilterExpressionMapOptionsStub.CreateIOptions(
                FilterKeyToFilterExpressionMapOptionsStub.Create(
                "AND",
                new Dictionary<string, FilterExpressionOptions>
                {
                    {
                        "CODE",
                        new FilterExpressionOptions
                        {
                            FilterExpressionKey = "SearchIn",
                            FilterExpressionValuesDelimiter = ","
                        }
                    }
                }));

        SearchFilterRequest request =
            SearchFilterRequestStub.Create("CODE", "A", "B");

        (Mock<ISearchFilterExpressionFactory> exprFactoryMock,
         Mock<ISearchFilterExpression> exprMock) =
            SearchFilterExpressionFactoryTestDouble.MockFor("SearchIn", "expr");

        (Mock<ILogicalOperatorFactory> opFactoryMock,
         Mock<ILogicalOperator> _) =
            LogicalOperatorFactoryTestDouble.MockFor("AND", " AND ");

        SearchFilterExpressionsBuilder builder =
            CreateBuilder(options, exprFactoryMock, opFactoryMock);

        // act
        string result = builder.BuildSearchFilterExpressions(new[] { request });

        // assert
        Assert.Equal("expr", result);
        Assert.Equal(",", request.FilterValuesDelimiter);

        exprFactoryMock.Verify(searchFilterExpressionFactory =>
            searchFilterExpressionFactory.CreateFilter("SearchIn"), Times.Once());
        exprMock.Verify(searchFilterExpression =>
            searchFilterExpression.GetFilterExpression(It.IsAny<SearchFilterRequest>()), Times.Once());
    }

    [Fact]
    public void GetFilterChainingLogicalOperator_WhenMissing_Throws_AndVerifiesNoFactoryCalls()
    {
        // arrange
        IOptions<FilterKeyToFilterExpressionMapOptions> options =
            FilterKeyToFilterExpressionMapOptionsStub.MissingChainingOperatorIOptions();

        Mock<ISearchFilterExpressionFactory> exprFactoryMock =
            SearchFilterExpressionFactoryTestDouble.Mock();

        Mock<ILogicalOperatorFactory> opFactoryMock =
            LogicalOperatorFactoryTestDouble.Mock();

        SearchFilterExpressionsBuilder builder =
            CreateBuilder(options, exprFactoryMock, opFactoryMock);

        // act / assert
        Assert.Throws<ArgumentException>(() =>
            builder.BuildSearchFilterExpressions([]));

        // verify
        exprFactoryMock.Verify(searchFilterExpressionFactory =>
            searchFilterExpressionFactory.CreateFilter(It.IsAny<string>()), Times.Never());

        opFactoryMock.Verify(logicalOperatorFactory =>
            logicalOperatorFactory.CreateLogicalOperator(It.IsAny<string>()), Times.Never());
    }
}
