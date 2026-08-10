using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;

internal sealed class SearchRequestBuilder
{
    private const string DefaultSearchIndexKey = "STUB_SEARCH_INDEX_KEY";
    private const string DefaultSearchKeywords = "search-term";
    private const string DefaultSortField = "UNDEFINED";

    private string _searchIndexKey = DefaultSearchIndexKey;
    private string _searchKeywords = DefaultSearchKeywords;
    private int _offset;
    private IList<FilterRequest>? _filterRequests;
    private SortOrder _sortOrder =
        new(
            sortField: DefaultSortField,
            sortDirection: "asc",
            validSortFields: [DefaultSortField]);

    public SearchRequestBuilder WithSearchIndexKey(string searchIndexKey)
    {
        _searchIndexKey = searchIndexKey;
        return this;
    }

    public SearchRequestBuilder WithSearchKeywords(string searchKeywords)
    {
        _searchKeywords = searchKeywords;
        return this;
    }

    public SearchRequestBuilder WithOffset(int offset)
    {
        _offset = offset;
        return this;
    }

    public SearchRequestBuilder WithFilterRequests(
        IList<FilterRequest> filterRequests)
    {
        _filterRequests = filterRequests;
        return this;
    }

    public SearchRequestBuilder WithSortOrder(SortOrder sortOrder)
    {
        _sortOrder = sortOrder;
        return this;
    }

    public SearchRequest Build()
    {
        return _filterRequests is null
            ? new SearchRequest(
                searchIndexKey: _searchIndexKey,
                searchKeywords: _searchKeywords,
                sortOrder: _sortOrder,
                offset: _offset)
            : new SearchRequest(
                searchIndexKey: _searchIndexKey,
                searchKeywords: _searchKeywords,
                filterRequests: _filterRequests,
                sortOrder: _sortOrder,
                offset: _offset);
    }

    public static SearchRequestBuilder Create() => new();
}
