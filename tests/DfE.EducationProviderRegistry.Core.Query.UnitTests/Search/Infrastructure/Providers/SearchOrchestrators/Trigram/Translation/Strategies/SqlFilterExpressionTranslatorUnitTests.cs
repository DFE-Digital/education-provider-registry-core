using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

public sealed class SqlFilterExpressionTranslatorUnitTests
{
    private sealed class TestProjection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private static EntityMetadata BuildMetadata() =>
        new EfEntityMetadataBuilder<TestProjection>()
            .WithDatabaseName("TestDb")
            .Build();

    private static string Translate(Expression<Func<TestProjection, bool>> expr)
    {
        EntityMetadata metadata = BuildMetadata();
        SqlFilterExpressionTranslator<TestProjection> translator = new();
        return translator.Translate(expr, metadata);
    }

    [Fact]
    public void EqualOperator_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection => projection.Id == 5);

        // assert
        Assert.Equal("(t.\"Id\" = 5)", sql);
    }

    [Fact]
    public void AndOperator_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                projection.Id == 5 &&
                projection.Age == 30);

        // assert
        Assert.Equal("((t.\"Id\" = 5) AND (t.\"Age\" = 30))", sql);
    }

    [Fact]
    public void OrOperator_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                projection.Id == 5 ||
                projection.Age == 30);

        // assert
        Assert.Equal("((t.\"Id\" = 5) OR (t.\"Age\" = 30))", sql);
    }

    [Fact]
    public void StringConstant_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                projection.Name == "Hooper");

        // assert
        Assert.Equal("(t.\"Name\" = 'Hooper')", sql);
    }

    [Fact]
    public void NullConstant_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                projection.Name == null);

        // assert
        Assert.Equal("(t.\"Name\" IS NULL)", sql);
    }

    [Fact]
    public void UnsupportedOperator_Throws()
    {
        // act/assert
        Assert.Throws<NotSupportedException>(() =>
            Translate(projection => projection.Age > 10));
    }

    [Fact]
    public void UnsupportedMemberExpression_Throws()
    {
        // act/assert
        Assert.Throws<NotSupportedException>(() =>
            Translate(projection => projection.Name.Length == 5));
    }

    [Fact]
    public void Composite_AndThenOr_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                (
                    projection.Id == 1 &&
                    projection.Age == 20
                ) ||
                projection.Name == "Hooper");

        // assert
        Assert.Equal(
            "(((t.\"Id\" = 1) AND (t.\"Age\" = 20)) OR (t.\"Name\" = 'Hooper'))", sql);
    }

    [Fact]
    public void Composite_AndWithNestedOr_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                projection.Id == 1 &&
                (
                    projection.Age == 20 ||
                    projection.Name == "Spencer")
                );

        // assert
        Assert.Equal(
            "((t.\"Id\" = 1) AND ((t.\"Age\" = 20) OR (t.\"Name\" = 'Spencer')))", sql);
    }

    [Fact]
    public void Composite_MultiNestedOrAnd_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                (projection.Id == 1 || projection.Id == 2) &&
                (projection.Name == "A" || projection.Name == "B")
        );

        // assert
        Assert.Equal(
            "(((t.\"Id\" = 1) OR (t.\"Id\" = 2)) AND ((t.\"Name\" = 'A') OR (t.\"Name\" = 'B')))",
            sql
        );
    }

    [Fact]
    public void Composite_DeeplyNested_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                (projection.Id == 1 && projection.Age == 10) ||
                (projection.Id == 2 && projection.Age == 20) ||
                (projection.Id == 3 && projection.Name == "X")
        );

        // assert
        Assert.Equal(
            "((((t.\"Id\" = 1) AND (t.\"Age\" = 10)) OR ((t.\"Id\" = 2) AND (t.\"Age\" = 20))) OR ((t.\"Id\" = 3) AND (t.\"Name\" = 'X')))", sql);
    }

    [Fact]
    public void Composite_MixedConstants_TranslatesCorrectly()
    {
        // arrange/act
        string sql =
            Translate(projection =>
                (projection.Name == null && projection.Id == 5) ||
                (projection.Name == "Test" && projection.Age == 30)
        );

        // assert
        Assert.Equal(
            "(((t.\"Name\" IS NULL) AND (t.\"Id\" = 5)) OR ((t.\"Name\" = 'Test') AND (t.\"Age\" = 30)))", sql);
    }

    [Fact]
    public void Composite_ParenthesesAreCorrect()
    {
        string sql = Translate(
            projection => projection.Id == 1 || (projection.Age == 20 && projection.Name == "A")
        );

        Assert.Equal(
            "((t.\"Id\" = 1) OR ((t.\"Age\" = 20) AND (t.\"Name\" = 'A')))",
            sql
        );
    }
}


