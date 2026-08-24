using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

public record AggregatedFacetResult(string FacetName, IReadOnlyCollection<FacetResult> Values);
