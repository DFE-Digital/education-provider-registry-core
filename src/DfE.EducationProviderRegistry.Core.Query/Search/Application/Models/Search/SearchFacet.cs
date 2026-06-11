namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Encapsulates the Faceted results returned by the <see cref="SearchUseCase"/> instance.
/// </summary>
public sealed class SearchFacet
{
    /// <summary>
    /// The facet (field) name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The collection of <see cref="FacetResult"/> instances.
    /// </summary>
    public IList<FacetResult> Results { get; }

    /// <summary>
    ///  Establishes an immutable <see cref="EstablishmentFacet"/> instance via the constructor arguments specified.
    /// </summary>
    /// <param name="facetName">
    /// The name of the facet on which to assign the prescribed results.
    /// </param>
    /// <param name="facetResults">
    /// The collection of <see cref="FacetResult"/> instances that carry
    /// the facet values and count of matched items found.
    /// </param>
    public SearchFacet(string facetName, IList<FacetResult> facetResults)
    {
        Name = facetName;
        Results = facetResults;
    }
}
