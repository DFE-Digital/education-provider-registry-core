using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SqlFilterExpressionTranslatorTestDouble
{
    public static Mock<ISqlFilterExpressionTranslator<TProjection>> Mock<TProjection>()
        where TProjection : class => new(MockBehavior.Strict);

    public static Mock<ISqlFilterExpressionTranslator<TProjection>> MockFor<TProjection>(
        EntityMetadata metadata,
        string response)
        where TProjection : class
    {
        Mock<ISqlFilterExpressionTranslator<TProjection>> translatorMock = Mock<TProjection>();

        translatorMock
            .Setup(translator =>
                translator.Translate(
                    It.IsAny<Expression<Func<TProjection, bool>>>(), metadata))
            .Returns(response);

        return translatorMock;
    }
}
