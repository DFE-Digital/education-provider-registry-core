using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal interface ISearchEstablishmentSeeder
{
    Task ClearAsync(CancellationToken ct = default);
    Task<SearchableEstablishments> SeedAsync(IReadOnlyCollection<Establishment> establishments, CancellationToken ct = default);
}

public sealed record SearchableEstablishments
{
    public required IReadOnlyCollection<Establishment> Establishments { get; init; }
}
