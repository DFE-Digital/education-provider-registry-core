using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

public sealed record SearchableEstablishments
{
    public required IReadOnlyCollection<Establishment> SearchTermMatches { get; init; }
}
