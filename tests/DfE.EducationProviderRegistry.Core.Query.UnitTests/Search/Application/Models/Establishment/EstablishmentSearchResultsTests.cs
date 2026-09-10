using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class SearchProviderResultsTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyCollection()
    {
        // arrange
        SearchProviderResults results = new();

        // assert
        Assert.NotNull(results.SearchResultCollection);
        Assert.Empty(results.SearchResultCollection);
        Assert.Equal(0, results.Count);
    }

    [Fact]
    public void Constructor_ShouldPopulateCollection_WhenListProvided()
    {
        // arrange
        List<SearchProviderResult> list =
        [
            SearchResultTestDouble.WithProviderIdentifier("10001"),
            SearchResultTestDouble.WithProviderIdentifier("10002")
        ];

        SearchProviderResults results = new(list);

        // assert
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results.SearchResultCollection.Count);
    }

    [Fact]
    public void Constructor_ShouldUseEmptyCollection_WhenNullProvided()
    {
        // arrange
        SearchProviderResults results = new(null!);

        // assert
        Assert.NotNull(results.SearchResultCollection);
        Assert.Empty(results.SearchResultCollection);
        Assert.Equal(0, results.Count);
    }

    [Fact]
    public void EstablishmentCollection_ShouldBeReadOnly()
    {
        // arrange
        List<SearchProviderResult> list =
        [
            SearchResultTestDouble.WithProviderIdentifier("10001")
        ];

        SearchProviderResults results = new(list);

        IReadOnlyCollection<SearchProviderResult> readOnly = results.SearchResultCollection;

        // assert
        Assert.Throws<NotSupportedException>(() =>
            ((IList<SearchProviderResult>)readOnly).Add(
                SearchResultTestDouble.WithProviderIdentifier("10002")));
    }

    [Fact]
    public void Constructor_ShouldCopyList_NotReferenceIt()
    {
        // arrange
        List<SearchProviderResult> list =
        [
            SearchResultTestDouble.WithProviderIdentifier("10001")
        ];

        SearchProviderResults results = new(list);

        list.Add(SearchResultTestDouble.WithProviderIdentifier("10002"));

        // assert
        Assert.Equal(1, results.Count);
    }

    [Fact]
    public void CreateEmpty_ShouldReturnEmptyInstance()
    {
        // arrange
        SearchProviderResults results = SearchProviderResults.CreateEmpty();

        // assert
        Assert.NotNull(results.SearchResultCollection);
        Assert.Empty(results.SearchResultCollection);
        Assert.Equal(0, results.Count);
    }
}
