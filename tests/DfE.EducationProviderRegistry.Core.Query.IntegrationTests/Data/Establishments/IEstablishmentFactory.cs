using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Builders;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;

public interface IEstablishmentFactory
{
    Task<CreatedEstablishmentResult> CreateAsync(
        Action<EstablishmentBuilder>? configure = null);
}
