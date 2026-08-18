using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;

internal sealed class SearchRequestBuilder
{
    private const string DefaultSearchKeywords = "search-term";
    private const string DefaultSortField = "UNDEFINED";

    private int _offset;
    private IList<FilterRequest>? _filterRequests;
    private IList<SearchTerm> _searchTerms = [];
    private SortOrder _sortOrder =
        new(
            sortField: DefaultSortField,
            sortDirection: "asc",
            validSortFields: [DefaultSortField]);

    public SearchRequestBuilder WithSearchTerm(string key, string term)
    {
        _searchTerms.Add(new(key, term));
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
                _searchTerms.AsReadOnly(),
                sortOrder: _sortOrder,
                offset: _offset)
            : new SearchRequest(
                _searchTerms.AsReadOnly(),
                filterRequests: _filterRequests,
                sortOrder: _sortOrder,
                offset: _offset);
    }

    public static SearchRequestBuilder Create() => new();
}
