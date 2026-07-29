using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;

public sealed class SearchServiceAdapterRequest
{
    public string? WhatTerm { get; }
    public string? WhereTerm { get; }
    public int Offset { get; }
    public int PageSize { get; }
    public IList<string> SearchFields { get; }
    public IList<string> Facets { get; }
    public IList<FilterRequest> SearchFilterRequests { get; }
    public SortOrder SortOrdering { get; }

    public SearchServiceAdapterRequest(
        string? whatTerm,
        string? whereTerm,
        IList<string> searchFields,
        SortOrder sortOrdering,
        IList<string>? facets = null,
        IList<FilterRequest>? searchFilterRequests = null,
        int offset = 0,
        int pageSize = 20)
    {
        SearchFields = searchFields?.Count > 0
            ? searchFields
            : throw new ArgumentException(
                $"A valid {nameof(searchFields)} argument must be provided.", nameof(searchFields));

        // TODO: this is just a noddy way of getting this to work, we need to fall back to the key/value collection Tony's provisoned
        WhatTerm = whatTerm;
        WhereTerm = whereTerm;

        SortOrdering = sortOrdering;
        Facets = facets ?? [];
        SearchFilterRequests = searchFilterRequests ?? [];
        Offset = offset;
        PageSize = pageSize;
    }

    public static SearchServiceAdapterRequest Create(
        string whatTerm,
        string whereTerm,
        IList<string> searchFields,
        IList<string> facets,
        SortOrder sortOrdering,
        IList<FilterRequest>? searchFilterRequests = null,
        int offset = 0, int pageSize = 20)
            => new(whatTerm, whereTerm, searchFields, sortOrdering, facets, searchFilterRequests, offset, pageSize);
}
