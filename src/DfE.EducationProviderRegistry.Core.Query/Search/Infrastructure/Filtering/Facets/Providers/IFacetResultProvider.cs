using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets.Providers;

public interface IFacetResultProvider
{
    IReadOnlyList<FacetResult> GetFacets(
        IEnumerable<FacetProjection> projections,
        IList<object> filterValues);
}
