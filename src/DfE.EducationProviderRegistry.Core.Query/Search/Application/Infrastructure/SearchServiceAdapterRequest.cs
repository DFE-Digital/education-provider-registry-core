using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;

public sealed class SearchServiceAdapterRequest
{
    public IReadOnlyCollection<SearchTerm?> SearchTerms { get; }
    public int Offset { get; }
    public int PageSize { get; }
    public IList<string> SearchFields { get; }
    public IList<string> Facets { get; }
    public IList<FilterRequest> SearchFilterRequests { get; }
    public SortOrder SortOrdering { get; }

    public SearchServiceAdapterRequest(
        IReadOnlyCollection<SearchTerm?> searchTerms,
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

        SearchTerms = searchTerms;
        SortOrdering = sortOrdering;
        Facets = facets ?? [];
        SearchFilterRequests = searchFilterRequests ?? [];
        Offset = offset;
        PageSize = pageSize;
    }

    public static SearchServiceAdapterRequest Create(
        IReadOnlyCollection<SearchTerm?> searchTerms,
        IList<string> searchFields,
        IList<string> facets,
        SortOrder sortOrdering,
        IList<FilterRequest>? searchFilterRequests = null,
        int offset = 0, int pageSize = 20)
            => new(searchTerms, searchFields, sortOrdering, facets, searchFilterRequests, offset, pageSize);
}
