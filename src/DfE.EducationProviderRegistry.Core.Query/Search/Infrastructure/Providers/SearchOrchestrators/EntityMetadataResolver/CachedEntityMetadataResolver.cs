using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

/// <summary>
/// Resolves and caches EF Core metadata for <typeparamref name="TProjection"/>.
/// Provides thread‑safe, one‑time reflection of table, schema, and primary key
/// information.
/// </summary>
/// <typeparam name="TProjection">The EF‑mapped entity type.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Usage",
    "CA2214:DoNotCallOverridableMethodsInConstructors",
    Justification = "Static cache is intentional and thread-safe.")]
internal sealed class CachedEntityMetadataResolver<TProjection> : IEntityMetadataResolver<TProjection>
    where TProjection : class
{
    private static readonly object Sync = new();
    private static EntityMetadata? Cached;

    /// <summary>
    /// Resolves EF Core metadata for <typeparamref name="TProjection"/> and caches it.
    /// </summary>
    /// <param name="db">The EF Core <see cref="DbContext"/>.</param>
    /// <returns>The resolved <see cref="EntityMetadata"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="db"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the entity is unmapped, lacks a table, or has no primary key.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when the entity uses a composite primary key.
    /// </exception>
    public EntityMetadata Resolve(DbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        if (Cached is not null)
        {
            return Cached;
        }

        lock (Sync)
        {
            if (Cached is not null)
            {
                return Cached;
            }

            IEntityType entityType = db.Model.FindEntityType(typeof(TProjection))
                ?? throw new InvalidOperationException(
                    $"Entity type '{typeof(TProjection).Name}' is not mapped in the DbContext.");

            string tableName = entityType.GetTableName()
                ?? throw new InvalidOperationException(
                    $"Entity '{typeof(TProjection).Name}' has no table mapping.");

            string schema = entityType.GetSchema() ?? "public";

            IKey key = entityType.FindPrimaryKey()
                ?? throw new InvalidOperationException(
                    $"Entity '{typeof(TProjection).Name}' has no primary key defined.");

            if (key.Properties.Count != 1)
            {
                throw new NotSupportedException(
                    $"Entity '{typeof(TProjection).Name}' has a composite primary key, which is not supported.");
            }

            IProperty primaryKeyProperty = key.Properties[0];
            string primaryKeyColumn = primaryKeyProperty.GetColumnName()
                ?? throw new InvalidOperationException(
                    $"Primary key column for '{typeof(TProjection).Name}' could not be resolved.");

            Cached = new EntityMetadata(
                entityType,
                schema,
                tableName,
                primaryKeyProperty,
                primaryKeyColumn);

            return Cached;
        }
    }
}
