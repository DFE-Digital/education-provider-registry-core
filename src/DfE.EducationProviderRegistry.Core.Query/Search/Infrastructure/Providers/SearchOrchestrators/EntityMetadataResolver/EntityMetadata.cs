using Microsoft.EntityFrameworkCore.Metadata;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

/// <summary>
/// Immutable snapshot of EF Core metadata for an entity, including schema,
/// table name, and primary key column information. Used by search orchestrators
/// to avoid repeated metadata reflection.
/// </summary>
public sealed record EntityMetadata
{
    /// <summary>
    /// The EF Core <see cref="IEntityType"/> describing the entity's runtime metadata.
    /// </summary>
    public IEntityType EntityType { get; }

    /// <summary>
    /// The database schema the entity is mapped to.
    /// </summary>
    public string Schema { get; }

    /// <summary>
    /// The database table name the entity is mapped to.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// The EF Core <see cref="IProperty"/> representing the entity's primary key.
    /// </summary>
    public IProperty PrimaryKeyProperty { get; }

    /// <summary>
    /// The database column name corresponding to the primary key property.
    /// </summary>
    public string PrimaryKeyColumn { get; }

    /// <summary>
    /// Creates an immutable metadata snapshot for an EF Core entity.
    /// </summary>
    /// <param name="entityType">The EF Core entity type.</param>
    /// <param name="schema">The resolved database schema.</param>
    /// <param name="tableName">The resolved database table name.</param>
    /// <param name="primaryKeyProperty">The EF Core primary key property.</param>
    /// <param name="primaryKeyColumn">The resolved primary key column name.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entityType"/> or <paramref name="primaryKeyProperty"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="schema"/>, <paramref name="tableName"/>,
    /// or <paramref name="primaryKeyColumn"/> is null, empty, or whitespace.
    /// </exception>
    public EntityMetadata(
        IEntityType entityType,
        string schema,
        string tableName,
        IProperty primaryKeyProperty,
        string primaryKeyColumn)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(primaryKeyProperty);

        if (string.IsNullOrWhiteSpace(schema))
        {
            throw new ArgumentException(
                "Schema cannot be null or empty.", nameof(schema));
        }

        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException(
                "Table name cannot be null or empty.", nameof(tableName));
        }

        if (string.IsNullOrWhiteSpace(primaryKeyColumn))
        {
            throw new ArgumentException(
                "Primary key column cannot be null or empty.", nameof(primaryKeyColumn));
        }

        EntityType = entityType;
        Schema = schema;
        TableName = tableName;
        PrimaryKeyProperty = primaryKeyProperty;
        PrimaryKeyColumn = primaryKeyColumn;
    }
}
