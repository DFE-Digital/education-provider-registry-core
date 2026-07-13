using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

public interface IEntityMetadataResolver<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Resolves the EF Core metadata for the specified projection type using the
    /// provided <see cref="DbContext"/> instance.
    /// </summary>
    /// <param name="db">
    /// The EF Core <see cref="DbContext"/> whose model is used to resolve metadata.
    /// </param>
    /// <returns>
    /// A fully populated <see cref="EntityMetadata"/> instance describing the entity's
    /// schema, table name, primary key property, and associated column mappings.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> is null.
    /// </exception>
    EntityMetadata Resolve(DbContext db);
}
