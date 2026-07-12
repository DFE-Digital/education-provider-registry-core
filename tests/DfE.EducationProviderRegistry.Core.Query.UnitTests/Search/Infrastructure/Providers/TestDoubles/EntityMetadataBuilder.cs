using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.TrigramSearchOrchestratorUnitTests;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EntityMetadataBuilder
{
    public static EntityMetadata BuildMetadata(DbContext db)
    {
        ModelBuilder modelBuilder = new();

        modelBuilder.Entity<TestEntity>()
            .ToTable("test_table", "public")
            .HasKey(entity => entity.Id);

        modelBuilder.Entity<TestEntity>()
            .Property(entity => entity.Name)
            .HasColumnName("name");

        IMutableModel? model = modelBuilder.Model;

        IEntityType entityType = (IEntityType)model.FindEntityType(typeof(TestEntity))!;
        IProperty pk = entityType.FindPrimaryKey()!.Properties[0];

        return new EntityMetadata(
            entityType,
            "public",
            "test_table",
            pk,
            "Id");
    }
}
