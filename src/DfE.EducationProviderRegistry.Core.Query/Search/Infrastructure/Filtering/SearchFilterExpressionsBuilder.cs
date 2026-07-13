using System.Collections.ObjectModel;
using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

/// <summary>
/// Composes and builds filter expression strings based on incoming request keys and values.
/// These keys are mapped to configured filter expression types, which determine how values
/// are formatted and combined into a final filter expression.
/// 
/// For example, given:
/// <code>
/// List&lt;SearchFilterRequest&gt; searchFilterRequests =
///     SearchFilterRequestBuilder.Create().BuildSearchFilterRequestsWith(
///         ("OFSTEDRATINGCODE", new List&lt;object&gt; { "2", "5", "9", "12" }),
///         ("RELIGIOUSCHARACTERCODE", new List&lt;object&gt; { "00", "02" }))
///            .BuildSearchFilterRequests();
/// </code>
/// 
/// And configuration:
/// <code>
/// "FilterKeyToFilterExpressionMapOptions": {
///     "FilterChainingLogicalOperator": "AndLogicalOperator",
///     "SearchFilterToExpressionMap": {
///         "RELIGIOUSCHARACTERCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter": ","
///         },
///         "OFSTEDRATINGCODE": {
///             "FilterExpressionKey": "SearchInFilterExpression",
///             "FilterExpressionValuesDelimiter": ","
///         }
///     }
/// }
/// </code>
/// 
/// The resulting filter expression string would be:
/// <code>
///     "search.in(OFSTEDRATINGCODE, '2,5,9,12') and search.in(RELIGIOUSCHARACTERCODE, '00,02')"
/// </code>
/// </summary>
internal sealed class SearchFilterExpressionsBuilder : ISearchFilterExpressionsBuilder
{
    private readonly ISearchFilterExpressionFactory _searchFilterExpressionFactory;
    private readonly ILogicalOperatorFactory _logicalOperatorFactory;
    private readonly StringBuilder _aggregatedSearchFilterExpression = new();
    private readonly FilterKeyToFilterExpressionMapOptions _filterKeyToFilterExpressionMapOptions;

    public SearchFilterExpressionsBuilder(
        ISearchFilterExpressionFactory searchFilterExpressionFactory,
        ILogicalOperatorFactory logicalOperatorFactory,
        IOptions<FilterKeyToFilterExpressionMapOptions> filterKeyToFilterExpressionMapOptions)
    {
        _searchFilterExpressionFactory = searchFilterExpressionFactory;
        _logicalOperatorFactory = logicalOperatorFactory;
        ArgumentNullException.ThrowIfNull(filterKeyToFilterExpressionMapOptions);
        _filterKeyToFilterExpressionMapOptions = filterKeyToFilterExpressionMapOptions.Value;
    }

    public string BuildSearchFilterExpressions(IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        IEnumerable<string> searchFilters = GetValidSearchFilterExpression(searchFilterRequests);
        ILogicalOperator logicalOperator = GetFilterChainingLogicalOperator();

        _aggregatedSearchFilterExpression.AppendJoin(logicalOperator.GetOperatorExpression(), searchFilters);

        return _aggregatedSearchFilterExpression.ToString();
    }

    private ReadOnlyCollection<string> GetValidSearchFilterExpression(IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        List<string> searchFilters = [];

        foreach (SearchFilterRequest searchFilterRequest in searchFilterRequests
            .Where(searchFilterRequest =>
                _filterKeyToFilterExpressionMapOptions
                    .SearchFilterToExpressionMap.ContainsKey(searchFilterRequest.FilterKey)))
        {
            FilterExpressionOptions filterExpressionOptions =
                _filterKeyToFilterExpressionMapOptions.SearchFilterToExpressionMap[searchFilterRequest.FilterKey];

            if (filterExpressionOptions.HasValuesDelimiter)
            {
                searchFilterRequest.SetFilterValuesDelimiter(filterExpressionOptions.FilterExpressionValuesDelimiter);
            }

            ISearchFilterExpression searchFilterExpression =
                _searchFilterExpressionFactory.CreateFilter(filterExpressionOptions.FilterExpressionKey);

            searchFilters.Add(searchFilterExpression.GetFilterExpression(searchFilterRequest));
        }

        return searchFilters.AsReadOnly();
    }

    private ILogicalOperator GetFilterChainingLogicalOperator()
    {
        string filterChainingLogicalOperatorKey =
            !string.IsNullOrWhiteSpace(_filterKeyToFilterExpressionMapOptions.FilterChainingLogicalOperator)
                ? _filterKeyToFilterExpressionMapOptions.FilterChainingLogicalOperator
                : throw new ArgumentException("Unable to assign a null or empty logical operator to the search expression chain.");

        return _logicalOperatorFactory.CreateLogicalOperator(filterChainingLogicalOperatorKey);
    }
}
