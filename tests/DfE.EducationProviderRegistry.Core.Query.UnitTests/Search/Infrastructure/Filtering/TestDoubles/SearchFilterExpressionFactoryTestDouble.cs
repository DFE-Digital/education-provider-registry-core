using System.Diagnostics.CodeAnalysis;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterExpressionFactoryTestDouble
{
    public static Mock<ISearchFilterSpecificationFactory<TProjection>> Mock<TProjection>()
        where TProjection : class
    {
        return new Mock<ISearchFilterSpecificationFactory<TProjection>>(MockBehavior.Strict);
    }

    public static (
        Mock<ISearchFilterSpecificationFactory<TProjection>> factory,
        Mock<ISearchFilter<TProjection>> expression
    ) MockFor<TProjection>(
        string filterKey,
        ISpecification<TProjection> spec)
        where TProjection : class
    {
        // Mock the filter expression itself
        Mock<ISearchFilter<TProjection>> exprMock =
            SearchFilterExpressionTestDouble.MockForSpecification<TProjection>(spec);

        exprMock
            .Setup((filter) =>
                filter.CreateSpecification(It.IsAny<SearchFilterRequest>()))
            .Returns(spec)
            .Verifiable();

        Mock<ISearchFilterSpecificationFactory<TProjection>> factoryMock = Mock<TProjection>();

        factoryMock
            .Setup((f) =>
                f.CreateFilter(
                    It.IsAny<string>(), It.IsAny<SearchFilterRequest>()))
            .Returns(spec)
            .Verifiable();

        return (factoryMock, exprMock);
    }
}
