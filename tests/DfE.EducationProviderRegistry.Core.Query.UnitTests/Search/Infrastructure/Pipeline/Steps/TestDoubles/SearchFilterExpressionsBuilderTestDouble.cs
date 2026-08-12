using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterExpressionsBuilderTestDouble
{
    public static Mock<ISearchFilterExpressionsBuilder<TProjection>> Mock<TProjection>()
        where TProjection : class => new(MockBehavior.Strict);

    public static Mock<ISearchFilterExpressionsBuilder<TProjection>> MockFor<TProjection>(
        Expression<Func<TProjection, bool>> expression)
        where TProjection : class
    {
        Mock<ISearchFilterExpressionsBuilder<TProjection>> builderMock = Mock<TProjection>();

        builderMock
            .Setup(searchFilterExpressionBuilder =>
                searchFilterExpressionBuilder.BuildSearchFilterExpression(
                    It.IsAny<IEnumerable<SearchFilterRequest>>()))
            .Returns(expression);

        return builderMock;
    }
}
