using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

[ExcludeFromCodeCoverage]
internal sealed class EfEntityMetadataBuilder<TProjection>
    where TProjection : class
{
    private string _databaseName;

    public EfEntityMetadataBuilder()
    {
        _databaseName = "TestDb";
    }

    public EfEntityMetadataBuilder<TProjection> WithDatabaseName(string name)
    {
        _databaseName = name;
        return this;
    }

    public EntityMetadata Build()
    {
        DbContextOptions<TestDbContext> options = CreateOptions();
        using TestDbContext context = new(options);

        IEntityType entityType = ResolveEntityType(context);
        string schema = ResolveSchema(entityType);
        string tableName = ResolveTableName(entityType);
        IProperty primaryKeyProperty = ResolvePrimaryKeyProperty(entityType);
        string primaryKeyColumn = ResolvePrimaryKeyColumn(primaryKeyProperty);

        return new EntityMetadata(
            entityType,
            schema,
            tableName,
            primaryKeyProperty,
            primaryKeyColumn);
    }

    private DbContextOptions<TestDbContext> CreateOptions()
    {
        DbContextOptionsBuilder<TestDbContext> builder = new();

        builder.UseInMemoryDatabase(_databaseName);

        return builder.Options;
    }

    private IEntityType ResolveEntityType(TestDbContext context)
    {
        IEntityType? entityTypeNullable =
            context.Model.FindEntityType(typeof(TProjection));

        return entityTypeNullable ?? throw new InvalidOperationException("EntityType not found.");
    }

    private string ResolveSchema(IEntityType entityType)
    {
        string? schemaNullable = entityType.GetSchema();
        return schemaNullable ?? "dbo";
    }

    private string ResolveTableName(IEntityType entityType) =>
        entityType.GetTableName() ??
            throw new InvalidOperationException("Table name not resolved.");


    private IProperty ResolvePrimaryKeyProperty(IEntityType entityType)
    {
        IKey? keyNullable = entityType.FindPrimaryKey() ??
            throw new InvalidOperationException("Primary key not found.");

        return keyNullable.Properties[0];
    }

    private string ResolvePrimaryKeyColumn(IProperty property) =>
        property.GetColumnName() ??
            throw new InvalidOperationException("Primary key column not resolved.");

    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        public DbSet<TProjection> Items => Set<TProjection>();
    }
}
