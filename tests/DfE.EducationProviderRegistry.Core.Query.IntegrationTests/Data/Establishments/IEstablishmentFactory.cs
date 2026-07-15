using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;

internal interface IEstablishmentFactory
{
    Task<Establishment> CreateAsync(
        Action<EstablishmentBuilder>? configure = null, CancellationToken ct = default);
}

