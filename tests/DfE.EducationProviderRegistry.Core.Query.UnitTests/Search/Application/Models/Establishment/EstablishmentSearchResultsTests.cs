using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class EstablishmentSearchResultsTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyCollection()
    {
        // arrange
        EstablishmentSearchResults results = new();

        // assert
        Assert.NotNull(results.EstablishmentCollection);
        Assert.Empty(results.EstablishmentCollection);
        Assert.Equal(0, results.Count);
    }

    [Fact]
    public void Constructor_ShouldPopulateCollection_WhenListProvided()
    {
        // arrange
        List<EstablishmentSearchResult> list =
        [
            EstablishmentSearchResultTestDouble.WithUrn("10001"),
            EstablishmentSearchResultTestDouble.WithUrn("10002")
        ];

        EstablishmentSearchResults results = new(list);

        // assert
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results.EstablishmentCollection.Count);
    }

    [Fact]
    public void Constructor_ShouldUseEmptyCollection_WhenNullProvided()
    {
        // arrange
        EstablishmentSearchResults results = new(null!);

        // assert
        Assert.NotNull(results.EstablishmentCollection);
        Assert.Empty(results.EstablishmentCollection);
        Assert.Equal(0, results.Count);
    }

    [Fact]
    public void EstablishmentCollection_ShouldBeReadOnly()
    {
        // arrange
        List<EstablishmentSearchResult> list =
        [
            EstablishmentSearchResultTestDouble.WithUrn("10001")
        ];

        EstablishmentSearchResults results = new(list);

        IReadOnlyCollection<EstablishmentSearchResult> readOnly = results.EstablishmentCollection;

        // assert
        Assert.Throws<NotSupportedException>(() =>
            ((IList<EstablishmentSearchResult>)readOnly).Add(
                EstablishmentSearchResultTestDouble.WithUrn("10002")));
    }

    [Fact]
    public void Constructor_ShouldCopyList_NotReferenceIt()
    {
        // arrange
        List<EstablishmentSearchResult> list =
        [
            EstablishmentSearchResultTestDouble.WithUrn("10001")
        ];

        EstablishmentSearchResults results = new(list);

        list.Add(EstablishmentSearchResultTestDouble.WithUrn("10002"));

        // assert
        Assert.Equal(1, results.Count);
    }

    [Fact]
    public void CreateEmpty_ShouldReturnEmptyInstance()
    {
        // arrange
        EstablishmentSearchResults results = EstablishmentSearchResults.CreateEmpty();

        // assert
        Assert.NotNull(results.EstablishmentCollection);
        Assert.Empty(results.EstablishmentCollection);
        Assert.Equal(0, results.Count);
    }
}
