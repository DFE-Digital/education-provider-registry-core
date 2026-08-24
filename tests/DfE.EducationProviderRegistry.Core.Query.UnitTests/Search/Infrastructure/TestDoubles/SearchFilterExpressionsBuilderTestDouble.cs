using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class SearchFilterExpressionsBuilderTestDouble
{
    public static Mock<ISearchFilterExpressionsBuilder<Establishment>> Mock()
    {
        Mock<ISearchFilterExpressionsBuilder<Establishment>> mock = new(MockBehavior.Strict);

        mock.Setup(expressionBuilder =>
            expressionBuilder.BuildSearchFilterExpression(
                It.IsAny<ReadOnlyCollection<SearchFilterRequest>>()))
            .Returns(establishment => true);

        return mock;
    }

    public static Mock<ISearchFilterExpressionsBuilder<Establishment>> MockFor(
        Expression<Func<Establishment, bool>> predicate)
    {
        Mock<ISearchFilterExpressionsBuilder<Establishment>> mock = new(MockBehavior.Strict);

        mock.Setup(expressionBuilder =>
            expressionBuilder.BuildSearchFilterExpression(
                It.IsAny<ReadOnlyCollection<SearchFilterRequest>>()))
            .Returns(predicate);

        return mock;
    }
}
