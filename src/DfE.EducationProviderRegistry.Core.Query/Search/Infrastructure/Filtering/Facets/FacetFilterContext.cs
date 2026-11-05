using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

public sealed record FacetFilterContext(string Facet, FilterRequest? Filter);
