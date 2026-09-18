using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data;

public interface ISeedDataHandler<TModel, TOutput>
{
    Task<TOutput> PersistAsync(TModel input, EducationProviderRegistryDbContext dbContext, CancellationToken ct = default);
}
