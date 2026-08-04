using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators;

public sealed class SearchOrchestratorContextUnitTests
{
    [Fact]
    public void Constructor_WithValidArguments_SetsPropertiesCorrectly()
    {
        // arrange
        string searchTerm = "academy";
        string searchColumn = "ProviderName";
        int pageSize = 25;
        int offset = 50;

        IReadOnlyList<SearchFilterRequest> filters =
        [
            new SearchFilterRequest("CODE", new object[] { "A" })
        ];

        // act
        SearchOrchestratorContext<DummyProjection> context =
            new()
            {
                SearchTerm = searchTerm,
                SearchColumn = searchColumn,
                PageSize = pageSize,
                Offset = offset,
                Filters = filters
            };

        // assert
        Assert.Equal(searchTerm, context.SearchTerm);
        Assert.Equal(searchColumn, context.SearchColumn);
        Assert.Equal(pageSize, context.PageSize);
        Assert.Equal(offset, context.Offset);
        Assert.Equal(filters, context.Filters);
    }

    [Fact]
    public void SearchColumn_DefaultsToEmptyString()
    {
        // arrange/act
        SearchOrchestratorContext<DummyProjection> context =
            new()
            {
                SearchTerm = "academy",
                PageSize = 10,
                Offset = 0
            };

        // assert
        Assert.Equal(string.Empty, context.SearchColumn);
    }

    [Fact]
    public void Filters_DefaultsToEmptyList()
    {
        // arrange/act
        SearchOrchestratorContext<DummyProjection> context =
            new()
            {
                SearchTerm = "academy",
                PageSize = 10,
                Offset = 0
            };

        // assert
        Assert.Empty(context.Filters);
    }

    [Fact]
    public void FilterExpression_DefaultsToTruePredicate()
    {
        // arrange
        SearchOrchestratorContext<DummyProjection> context =
            new()
            {
                SearchTerm = "academy",
                PageSize = 10,
                Offset = 0
            };

        // act
        Func<DummyProjection, bool> compiled = context.FilterExpression.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 123 }));
    }

    [Fact]
    public void Filters_AreImmutable()
    {
        // arrange
        IReadOnlyList<SearchFilterRequest> filters =
        [
            new SearchFilterRequest("CODE", new object[] { "A" })
        ];

        SearchOrchestratorContext<DummyProjection> context =
            new()
            {
                SearchTerm = "academy",
                PageSize = 10,
                Offset = 0,
                Filters = filters
            };

        // act
        IReadOnlyList<SearchFilterRequest> returned = context.Filters;

        // assert
        Assert.Throws<InvalidCastException>(() =>
        {
            ((List<SearchFilterRequest>)returned).Add(
                new SearchFilterRequest("X", new object[] { "Y" }));
        });
    }

    [Fact]
    public void Record_Immutability_ProducesNewInstanceOnWithExpression()
    {
        // arrange
        SearchOrchestratorContext<DummyProjection> original =
            new()
            {
                SearchTerm = "academy",
                SearchColumn = "ProviderName",
                PageSize = 25,
                Offset = 0,
                Filters = []
            };

        // act
        SearchOrchestratorContext<DummyProjection> updated =
            original with { PageSize = 50 };

        // assert
        Assert.Equal(25, original.PageSize);
        Assert.Equal(50, updated.PageSize);
        Assert.NotSame(original, updated);
    }
}
