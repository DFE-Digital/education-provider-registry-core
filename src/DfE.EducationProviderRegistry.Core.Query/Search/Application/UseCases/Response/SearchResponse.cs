using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

public sealed class SearchResponse
{
    /// <summary>
    /// Initializes a new response with the specified search status and total result count.
    /// </summary>
    /// <param name="establishmentSearchResults">
    /// The results of the establishment search.
    /// </param>
    /// <param name="facetedResults">
    /// The faceted aggregation results used for UI filtering.
    /// </param>
    /// <param name="totalNumberOfResults">
    /// The total number of matching learner records found. Defaults to zero if null or negative.
    /// </param>
    public SearchResponse(
        EstablishmentSearchResults establishmentSearchResults,
        SearchFacets? facetedResults,
        int totalNumberOfResults)
    {
        EstablishmentResults = establishmentSearchResults;
        FacetedResults = facetedResults;
        TotalNumberOfResults = totalNumberOfResults;
    }

    /// <summary>
    /// Gets the collection of learner search results returned by the query.
    /// </summary>
    public EstablishmentSearchResults? EstablishmentResults { get; }

    /// <summary>
    /// Gets the faceted aggregation results used for UI filtering, analytics, or navigation.
    /// </summary>
    public SearchFacets? FacetedResults { get; }

    /// <summary>
    /// Gets the overall status of the search execution.
    /// </summary>
    public SearchResponseStatus Status =>
        TotalNumberOfResults > 0
            ? SearchResponseStatus.Success
            : SearchResponseStatus.NoResultsFound;

    public int TotalNumberOfResults { get; }
}
