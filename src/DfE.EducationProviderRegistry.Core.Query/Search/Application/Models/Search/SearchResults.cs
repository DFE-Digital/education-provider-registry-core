namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Encapsulates the search results and associated facet
/// types that make up the response from the underlying search system.
/// </summary>
public class SearchResults<TResults, TFacetResults>
{
    /// <summary>
    /// The <see cref="TFacetResults"/> returned from the search
    /// which encapsulates the underlying search results collection
    /// that is built from the underlying search response.
    /// </summary>
    public TResults? Results { get; init; }

    /// <summary>
    /// The <see cref="TFacetResults"/> returned from the search
    /// which encapsulates the underlying facets collection
    /// that is built from the underlying search response.
    /// </summary>
    public TFacetResults? FacetResults { get; init; }

    /// <summary>
    /// Gets the total number of results returned from the underlying search system.
    /// </summary>
    public int TotalCount { get; init; }
}
