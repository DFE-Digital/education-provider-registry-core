namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Encapsulates the <see cref="EstablishmentResults"/> and <see cref="Facets"/>
/// types that make up the response from the underlying search system.
/// </summary>
public class SearchResults<TResults, TFacetResults>
{
    /// <summary>
    /// The <see cref="TFacetResults"/> returned from the Establishment search
    /// which encapsulates the underlying <see cref="Establishment"/> collection
    /// that is built from the underlying search response.
    /// </summary>
    public TResults? Results { get; init; }

    /// <summary>
    /// The <see cref="TFacetResults"/> returned from the Establishment search
    /// which encapsulates the underlying <see cref="EstablishmentFacet"/> collection
    /// that is built from the underlying search response.
    /// </summary>
    public TFacetResults? FacetResults { get; init; }

    /// <summary>
    ///     Gets the total number of results returned from the underlying search system.
    /// </summary>
    public int TotalCount { get; init; }

}
