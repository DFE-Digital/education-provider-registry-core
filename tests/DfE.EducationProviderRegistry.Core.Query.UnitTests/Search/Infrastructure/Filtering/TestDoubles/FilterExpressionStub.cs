using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal sealed class FilterExpressionStub : ISearchFilterExpression
{
    private readonly string _result;

    public FilterExpressionStub(string result)
    {
        _result = result;
    }

    public string GetFilterExpression(SearchFilterRequest request, string filterExpressionTarget) => _result;
}
