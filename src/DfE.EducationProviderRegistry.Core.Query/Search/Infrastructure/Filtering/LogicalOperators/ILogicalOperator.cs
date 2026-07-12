namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;

/// <summary>
/// Provides an abstraction over which to derive Azure AI OData logical operator.
/// OData logical operator filter expression are Boolean expressions that evaluate to true or false.
/// For more information on OData filter  syntax in Azure AI search please see,
/// <a href="https://learn.microsoft.com/en-us/azure/search/search-query-odata-logical-operators"> OData logical operators</a>.
/// </summary>
public interface ILogicalOperator
{
    string GetOperatorExpression();
}
