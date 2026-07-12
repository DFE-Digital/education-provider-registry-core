using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

[ExcludeFromCodeCoverage]
public sealed class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class CompositeKeyEntity
{
    public int A { get; set; }
    public int B { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class TestDbContext : DbContext
{
    public TestDbContext()
    {
    }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    public DbSet<CompositeKeyEntity> CompositeKeyEntities => Set<CompositeKeyEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase("MetadataTests");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>()
            .ToTable("TestTable", "customschema")
            .HasKey(testEntity => testEntity.Id);

        modelBuilder.Entity<CompositeKeyEntity>()
            .ToTable("CompositeTable")
            .HasKey(compositeKeyEntity =>
                new { compositeKeyEntity.A, compositeKeyEntity.B });
    }
}
