using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class SearchFilterExpressionsBuilderTestDouble
{
    public static Mock<ISearchFilterExpressionsBuilder<SearchAggregate>> Mock()
    {
        Mock<ISearchFilterExpressionsBuilder<SearchAggregate>> mock = new(MockBehavior.Strict);

        Expression<Func<SearchAggregate, bool>> trueExpr = sa => true;

        mock.Setup(builder =>
            builder.BuildSearchFilterExpression(It.IsAny<IEnumerable<SearchFilterRequest>>()))
            .Returns(trueExpr);

        mock.Setup(builder =>
            builder.BuildSearchFilterExpression(It.IsAny<ReadOnlyCollection<SearchFilterRequest>>()))
            .Returns(trueExpr);

        return mock;
    }

    public static Mock<ISearchFilterExpressionsBuilder<SearchAggregate>> MockFor(
        Expression<Func<SearchAggregate, bool>> predicate)
    {
        Mock<ISearchFilterExpressionsBuilder<SearchAggregate>> mock = new(MockBehavior.Strict);

        mock.Setup(expressionBuilder =>
            expressionBuilder.BuildSearchFilterExpression(
                It.IsAny<IEnumerable<SearchFilterRequest>>()))
            .Returns(predicate);

        return mock;
    }
}
