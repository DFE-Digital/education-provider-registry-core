using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Insert;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;

internal sealed class EstablishmentFactory : IEstablishmentFactory
{
    private readonly IInsertEstablishmentHandler _insertEstablishmentHandler;

    public EstablishmentFactory(IInsertEstablishmentHandler insertEstablishmentHandler)
    {
        ArgumentNullException.ThrowIfNull(insertEstablishmentHandler);
        _insertEstablishmentHandler = insertEstablishmentHandler;
    }

    public async Task<Establishment> CreateAsync(
        Action<EstablishmentBuilder>? configure = null,
        CancellationToken ct = default)
    {
        EstablishmentBuilder builder = new();

        configure?.Invoke(builder);

        Establishment establishment = builder.Build();

        await _insertEstablishmentHandler.InsertAsync(
            [establishment],
            ct);

        return establishment;
    }
}
