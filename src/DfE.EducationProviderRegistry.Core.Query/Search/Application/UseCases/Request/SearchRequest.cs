using System.ComponentModel.DataAnnotations;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

/// <summary>
/// Represents a search request for search querying configured indexes,
/// optionally scoped with filters and offset for pagination.
/// </summary>
public sealed class SearchRequest : IUseCaseRequest<UseCaseResponse<SearchResponse>>
{
    /// <summary>
    /// Initializes a basic search request with keyword(s) and optional offset.
    /// </summary>
    /// <param name="searchKeywords">The keyword(s) used to query data.</param>
    /// <param name="offset">Offset for pagination (defaults to 0).</param>
    /// <exception cref="ArgumentException">Thrown if searchKeyword is null or empty.</exception>
    public SearchRequest(IReadOnlyCollection<SearchTerm?> searchTerms, SortOrder sortOrder, int offset = 0)
    {
        SearchTerms = searchTerms;
        SortOrder = sortOrder ?? throw new ArgumentNullException(nameof(sortOrder));
        Offset = offset;
    }

    /// <summary>
    /// Initializes a filtered search request.
    /// </summary>
    /// <param name="searchKeywords">The search keyword(s).</param>
    /// <param name="filterRequests">A list of filter criteria.</param>
    /// <param name="offset">Offset for pagination (defaults to 0).</param>
    public SearchRequest(
        IReadOnlyCollection<SearchTerm?> searchTerms,
        IList<FilterRequest> filterRequests,
        SortOrder sortOrder,
        int offset = 0) : this(searchTerms, sortOrder, offset)
    {
        FilterRequests = filterRequests ??
            throw new ArgumentNullException(nameof(filterRequests));
    }


    public IReadOnlyCollection<SearchTerm?> SearchTerms { get; }

    /// <summary>
    /// The offset used for paging through search results.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Offset must be non-negative.")]
    public int Offset { get; }

    /// <summary>
    /// Optional filters used to narrow down the search results.
    /// </summary>
    public IList<FilterRequest>? FilterRequests { get; }

    /// <summary>
    /// Specifies the order in which search results should be sorted.
    /// </summary>
    public SortOrder SortOrder { get; }
}

