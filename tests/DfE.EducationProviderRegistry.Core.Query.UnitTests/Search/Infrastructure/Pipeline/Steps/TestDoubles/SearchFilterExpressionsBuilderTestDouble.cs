using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal class SearchFilterExpressionsBuilderTestDouble
{
    public static Mock<ISearchFilterExpressionsBuilder> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<ISearchFilterExpressionsBuilder>
        MockFor(string filterExpression)
    {
        Mock<ISearchFilterExpressionsBuilder> searchFilterBuilderMock = Mock();

        searchFilterBuilderMock
            .Setup(filterBuilder =>
                filterBuilder.BuildSearchFilterExpressions(
                    It.IsAny<IReadOnlyList<SearchFilterRequest>>()))
                        .Returns(filterExpression);

        return searchFilterBuilderMock;
    }
}
