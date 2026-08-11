using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterRequestStub
{
    public static SearchFilterRequest Default() => Create(key: "STUB-FILTER-KEY", values: ["STUB-FILTERVALUE-1"]);
    public static SearchFilterRequest Create(string key, object[] values)
        => new(key, values);
}
