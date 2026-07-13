using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

public sealed class EntityMetadataUnitTests
{
    private static (IEntityType entityType, IProperty pkProperty) BuildEfMetadata()
    {
        DbContextOptions<TestDbContext> options =
            new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("EntityMetadataTests")
                .Options;

        TestDbContext db = new(options);

        IEntityType entityType = db.Model.FindEntityType(typeof(TestEntity))!;
        IProperty pk = entityType.FindPrimaryKey()!.Properties[0];

        db.Dispose();
        return (entityType, pk);
    }

    [Fact]
    public void Constructor_Succeeds_WhenAllArgumentsAreValid()
    {
        // arrange
        (IEntityType entityType, IProperty pk) metadataTuple = BuildEfMetadata();
        IEntityType entityType = metadataTuple.entityType;
        IProperty pk = metadataTuple.pk;

        // act
        EntityMetadata metadata = new(
            entityType,
            "custom_schema",
            "test_table",
            pk,
            "id");

        // assert
        Assert.Equal(entityType, metadata.EntityType);
        Assert.Equal("custom_schema", metadata.Schema);
        Assert.Equal("test_table", metadata.TableName);
        Assert.Equal(pk, metadata.PrimaryKeyProperty);
        Assert.Equal("id", metadata.PrimaryKeyColumn);
    }

    [Fact]
    public void Constructor_Throws_WhenEntityTypeIsNull()
    {
        // arrange
        (IEntityType _, IProperty pk) metadataTuple = BuildEfMetadata();
        IProperty pk = metadataTuple.pk;

        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            new EntityMetadata(
                null!,
                "schema",
                "table",
                pk,
                "id"));
    }

    [Fact]
    public void Constructor_Throws_WhenPrimaryKeyPropertyIsNull()
    {
        // arrange
        (IEntityType entityType, IProperty _) metadataTuple = BuildEfMetadata();
        IEntityType entityType = metadataTuple.entityType;

        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            new EntityMetadata(
                entityType,
                "schema",
                "table",
                null!,
                "id"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenSchemaIsInvalid(string? schema)
    {
        // arrange
        (IEntityType entityType, IProperty pk) metadataTuple = BuildEfMetadata();
        IEntityType entityType = metadataTuple.entityType;
        IProperty pk = metadataTuple.pk;

        // act / assert
        Assert.Throws<ArgumentException>(() =>
            new EntityMetadata(
                entityType,
                schema!,
                "table",
                pk,
                "id"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenTableNameIsInvalid(string? tableName)
    {
        // arrange
        (IEntityType entityType, IProperty pk) metadataTuple = BuildEfMetadata();
        IEntityType entityType = metadataTuple.entityType;
        IProperty pk = metadataTuple.pk;

        // act / assert
        Assert.Throws<ArgumentException>(() =>
            new EntityMetadata(
                entityType,
                "schema",
                tableName!,
                pk,
                "id"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenPrimaryKeyColumnIsInvalid(string? pkColumn)
    {
        // arrange
        (IEntityType entityType, IProperty pk) metadataTuple = BuildEfMetadata();
        IEntityType entityType = metadataTuple.entityType;
        IProperty pk = metadataTuple.pk;

        // act / assert
        Assert.Throws<ArgumentException>(() =>
            new EntityMetadata(
                entityType,
                "schema",
                "table",
                pk,
                pkColumn!));
    }

    [Fact]
    public void Record_Equality_Works_AsExpected()
    {
        // arrange
        (IEntityType entityType, IProperty pk) metadataTuple = BuildEfMetadata();
        IEntityType entityType = metadataTuple.entityType;
        IProperty pk = metadataTuple.pk;

        EntityMetadata a = new(entityType, "schema", "table", pk, "id");
        EntityMetadata b = new(entityType, "schema", "table", pk, "id");

        // act / assert
        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void ToString_ReturnsMeaningfulRepresentation()
    {
        // arrange
        (IEntityType entityType, IProperty pk) metadataTuple = BuildEfMetadata();
        IEntityType entityType = metadataTuple.entityType;
        IProperty pk = metadataTuple.pk;

        EntityMetadata metadata = new(
            entityType,
            "custom_schema",
            "test_table",
            pk,
            "id");

        // act
        string result = metadata.ToString();

        // assert
        Assert.Contains("TestEntity", result);
        Assert.Contains("custom_schema", result);
        Assert.Contains("test_table", result);
        Assert.Contains("id", result);
    }
}
