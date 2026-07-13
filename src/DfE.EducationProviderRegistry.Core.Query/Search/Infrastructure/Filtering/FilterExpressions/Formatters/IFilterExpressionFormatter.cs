namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Formatters;

public interface IFilterExpressionFormatter
{
    void SetExpressionParamsSeparator(string separator);

    string CreateFilterCriteriaPlaceholders(object[] filterCriteria);

    string CreateFormattedExpression(string expressionFormat, params object[] filterCriteria);
}
