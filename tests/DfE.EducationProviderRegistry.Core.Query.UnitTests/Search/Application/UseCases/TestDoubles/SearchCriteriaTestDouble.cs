using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
public static class SearchCriteriaTestDouble
{
    public static SearchCriteria Stub() => new()
    {
        Facets = ["FIELD1", "FIELD2", "FIELD3"],       // Simulated facet fields
        SearchFields = ["FACET1", "FACET2", "FACET3"]  // Simulated searchable fields
    };
}
