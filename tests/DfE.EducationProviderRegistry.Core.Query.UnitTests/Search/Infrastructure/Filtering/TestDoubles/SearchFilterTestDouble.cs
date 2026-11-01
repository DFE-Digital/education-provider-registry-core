using System.Diagnostics.CodeAnalysis;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterTestDouble
{
    public static Mock<ISearchFilter<TProjection>> Mock<TProjection>()
        where TProjection : class => new(MockBehavior.Strict);

    public static Mock<ISearchFilter<TProjection>> MockForSpecification<TProjection>(ISpecification<TProjection> spec) where TProjection : class
    {
        Mock<ISearchFilter<TProjection>> exprMock = Mock<TProjection>();

        exprMock
            .Setup((filter) =>
                filter.CreateSpecification(It.IsAny<SearchFilterRequest>()))
            .Returns(spec)
            .Verifiable();

        return exprMock;
    }

    public static FakeSearchFilter<TProjection> Fake<TProjection>() => new();
}

internal sealed class FakeSearchFilter<TProjection> : ISearchFilter<TProjection>
{
    public SearchFilterRequest? Request { get; private set; }

    public ISpecification<TProjection> CreateSpecification(SearchFilterRequest request)
    {
        Request = request;

        return SpecificationTestDoubles.True<TProjection>();
    }
}
