namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// The search criteria used by the various use-cases requiring access to the search service.
/// which is set using the configuration settings defined (via IOptions pattern).
/// </summary>
public class SearchCriteria
{
    /// <summary>
    /// The collection of fields in the underlying collection to search over.
    /// </summary>
    public IList<string> SearchFields { get; set; } = [];

    /// <summary>
    /// The collection of facets to apply in the search request.
    /// </summary>
    public IList<string> Facets { get; set; } = [];
}
