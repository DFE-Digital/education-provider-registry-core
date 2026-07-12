using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchOrchestratorTestDouble
{
    public static Mock<ISearchOrchestrator<Establishment>> Mock() =>
        new(MockBehavior.Strict);
}
