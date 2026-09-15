using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class SearchProviderResultsTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyCollection()
    {
        // arrange
        SearchAggregateResults results = new();

        // assert
        Assert.NotNull(results.SearchResultCollection);
        Assert.Empty(results.SearchResultCollection);
        Assert.Equal(0, results.Count);
    }

    [Fact]
    public void Constructor_ShouldPopulateCollection_WhenListProvided()
    {
        // arrange
        List<SearchAggregateResult> list =
        [
            SearchResultTestDouble.WithProviderIdentifier("10001"),
            SearchResultTestDouble.WithProviderIdentifier("10002")
        ];

        SearchAggregateResults results = new(list);

        // assert
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results.SearchResultCollection.Count);
    }

    [Fact]
    public void Constructor_ShouldUseEmptyCollection_WhenNullProvided()
    {
        // arrange
        SearchAggregateResults results = new(null!);

        // assert
        Assert.NotNull(results.SearchResultCollection);
        Assert.Empty(results.SearchResultCollection);
        Assert.Equal(0, results.Count);
    }

    [Fact]
    public void EstablishmentCollection_ShouldBeReadOnly()
    {
        // arrange
        List<SearchAggregateResult> list =
        [
            SearchResultTestDouble.WithProviderIdentifier("10001")
        ];

        SearchAggregateResults results = new(list);

        IReadOnlyCollection<SearchAggregateResult> readOnly = results.SearchResultCollection;

        // assert
        Assert.Throws<NotSupportedException>(() =>
            ((IList<SearchAggregateResult>)readOnly).Add(
                SearchResultTestDouble.WithProviderIdentifier("10002")));
    }

    [Fact]
    public void Constructor_ShouldCopyList_NotReferenceIt()
    {
        // arrange
        List<SearchAggregateResult> list =
        [
            SearchResultTestDouble.WithProviderIdentifier("10001")
        ];

        SearchAggregateResults results = new(list);

        list.Add(SearchResultTestDouble.WithProviderIdentifier("10002"));

        // assert
        Assert.Equal(1, results.Count);
    }

    [Fact]
    public void CreateEmpty_ShouldReturnEmptyInstance()
    {
        // arrange
        SearchAggregateResults results = SearchAggregateResults.CreateEmpty();

        // assert
        Assert.NotNull(results.SearchResultCollection);
        Assert.Empty(results.SearchResultCollection);
        Assert.Equal(0, results.Count);
    }
}
