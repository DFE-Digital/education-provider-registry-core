namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

public interface ISearchFilterExpression
{
    string GetFilterExpression(SearchFilterRequest searchFilterRequest, string filterExpressionTarget);
}
