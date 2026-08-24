using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.Request;

public sealed class SearchRequestTests
{
    [Fact]
    public void Constructor_WithNullSortOrder_ThrowsArgumentNullException()
    {
        // arrange
        IReadOnlyCollection<SearchTerm?> searchTerms = [SearchTermTestDouble.Stub()];

        Func<SearchRequest> construct = () =>
            new SearchRequest(searchTerms, null!);

        // act / assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WithNullFilterRequests_ThrowsArgumentNullException()
    {
        // arrange
        IReadOnlyCollection<SearchTerm?> searchTerms = [SearchTermTestDouble.Stub()];
        SortOrder sortOrder = SortOrderTestDouble.Stub();

        Func<SearchRequest> construct = () =>
            new SearchRequest(
                searchTerms,
                null!,
                sortOrder);

        // act / assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WithFilterParam_PopulatesFilterRequests()
    {
        // arrange
        List<FilterRequest> filterRequests = [FilterRequestTestDouble.Fake()];
        IReadOnlyCollection<SearchTerm?> searchTerms = [SearchTermTestDouble.Stub()];
        SortOrder sortOrder = SortOrderTestDouble.Stub();

        // act
        SearchRequest request = new(
            searchTerms: searchTerms,
            filterRequests: filterRequests,
            sortOrder: sortOrder);

        // assert
        Assert.NotNull(request.SearchTerms);
        Assert.NotNull(request.FilterRequests);
        Assert.NotNull(request.SortOrder);

        Assert.Equal(searchTerms.Count, request.SearchTerms.Count);

        FilterRequest expected = filterRequests[0];

        FilterRequest? actual =
            request.FilterRequests!
                .FirstOrDefault(fr => fr.FilterName == expected.FilterName);

        Assert.NotNull(actual);
        Assert.Equal(expected.FilterValues.Count, actual!.FilterValues.Count);

        Assert.True(actual.FilterValues.CollectionsMatch(
            expected.FilterValues,
            (expected, actual) => Equals(expected, actual)));
    }

    [Fact]
    public void Constructor_WithNoFilterParam_HasFilterRequestsNull()
    {
        // arrange
        IReadOnlyCollection<SearchTerm?> searchTerms = SearchTermTestDouble.StubSingle();
        SortOrder sortOrder = SortOrderTestDouble.Stub();

        // act
        SearchRequest request = new(
            searchTerms: searchTerms,
            sortOrder: sortOrder);

        // assert
        Assert.NotNull(request.SearchTerms);
        Assert.NotNull(request.SortOrder);
        Assert.Null(request.FilterRequests);
    }

    [Fact]
    public void Constructor_WithSetOffsetValue_AssignsCorrectPropertyValue()
    {
        // arrange
        IReadOnlyCollection<SearchTerm?> searchTerms = SearchTermTestDouble.StubSingle();
        SortOrder sortOrder = SortOrderTestDouble.Stub();
        const int offset = 10;

        // act
        SearchRequest request = new(
            searchTerms: searchTerms,
            sortOrder: sortOrder,
            offset: offset);

        // assert
        Assert.NotNull(request.SearchTerms);
        Assert.NotNull(request.SortOrder);
        Assert.Equal(offset, request.Offset);
    }

    [Fact]
    public void Constructor_WithDefaultOffsetValue_AssignsDefaultPropertyValue()
    {
        // arrange
        IReadOnlyCollection<SearchTerm?> searchTerms = SearchTermTestDouble.StubSingle();
        SortOrder sortOrder = SortOrderTestDouble.Stub();

        // act
        SearchRequest request = new(
            searchTerms: searchTerms,
            sortOrder: sortOrder);

        // assert
        Assert.NotNull(request.SearchTerms);
        Assert.NotNull(request.SortOrder);
        Assert.Equal(0, request.Offset);
    }

    [Fact]
    public void Constructor_AssignsSearchTerms()
    {
        // arrange
        IReadOnlyCollection<SearchTerm?> searchTerms = SearchTermTestDouble.StubMultiple();

        SortOrder sortOrder = SortOrderTestDouble.Stub();

        // act
        SearchRequest request = new(
            searchTerms: searchTerms,
            sortOrder: sortOrder);

        // assert
        Assert.Same(searchTerms, request.SearchTerms);
    }
}
