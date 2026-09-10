using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Builders;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data;

internal interface IEstablishmentFactory
{
    Task<Establishment> CreateAsync(
        Action<EstablishmentBuilder>? configure = null, CancellationToken ct = default);
}
