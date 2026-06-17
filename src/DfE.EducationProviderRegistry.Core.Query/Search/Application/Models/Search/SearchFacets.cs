namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Encapsulates the aggregation of facets returned from the underlying search system.
/// </summary>
public sealed class SearchFacets
{
    /// <summary>
    /// The readonly collection of facets derived from the underlying search mechanism.
    /// </summary>
    public IReadOnlyCollection<SearchFacet> Facets => _searchFacets.AsReadOnly();

    private readonly List<SearchFacet> _searchFacets;

    /// <summary>
    ///  Default constructor initialises a new readonly
    ///  collection of <see cref="SearchFacet"/> instances.
    /// </summary>
    public SearchFacets()
    {
        _searchFacets = [];
    }

    /// <summary>
    ///  Establishes an immutable collection of <see cref="SearchFacet"/>
    ///  instance via the constructor argument specified.
    /// </summary>
    /// <param name="searchFacets">
    /// Collection of configured <see cref="SearchFacet"/> instances.
    /// </param>
    public SearchFacets(IEnumerable<SearchFacet> searchFacets)
    {
        _searchFacets = [.. searchFacets];
    }
}
