using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

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
            new SearchFilterRequest("CODE", ["A"])
        ];

        // act
        SearchOrchestratorContext context = new()
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
        // act
        SearchOrchestratorContext context = new()
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
        // act
        SearchOrchestratorContext context = new()
        {
            SearchTerm = "academy",
            PageSize = 10,
            Offset = 0
        };

        // assert
        Assert.Empty(context.Filters);
    }

    [Fact]
    public void Filters_AreImmutable()
    {
        // arrange
        IReadOnlyList<SearchFilterRequest> filters =
        [
            new SearchFilterRequest("CODE", ["A"])
        ];

        SearchOrchestratorContext context = new()
        {
            SearchTerm = "academy",
            PageSize = 10,
            Offset = 0,
            Filters = filters
        };

        IReadOnlyList<SearchFilterRequest> returned = context.Filters;

        // act/assert
        Assert.Throws<InvalidCastException>(() =>
        {
            // attempt to mutate by forcing a cast to List<T>
            ((List<SearchFilterRequest>)returned).Add(
                new SearchFilterRequest("X", ["Y"]));
        });
    }

    [Fact]
    public void Record_Immutability_ProducesNewInstanceOnWithExpression()
    {
        // arrange
        SearchOrchestratorContext original = new()
        {
            SearchTerm = "academy",
            SearchColumn = "ProviderName",
            PageSize = 25,
            Offset = 0,
            Filters = []
        };

        // act
        SearchOrchestratorContext updated = original with { PageSize = 50 };

        // assert
        Assert.Equal(25, original.PageSize);
        Assert.Equal(50, updated.PageSize);
        Assert.NotSame(original, updated);
    }
}
