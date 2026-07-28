using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

/// <summary>
/// Resolves and composes typed filter expressions for <typeparamref name="TProjection"/>
/// using the configured filter‑key map and logical operator. Produces a single
/// provider‑agnostic predicate expression suitable for translation by the search pipeline.
/// </summary>
/// <typeparam name="TProjection">The projection or entity type.</typeparam>
public sealed class SearchFilterExpressionsBuilder<TProjection>
    : ISearchFilterExpressionsBuilder<TProjection>
    where TProjection : class
{
    private readonly ISearchFilterFactory<TProjection> _filterExpressionFactory;
    private readonly FilterKeyToFilterExpressionMapOptions _filterKeyMapOptions;

    public SearchFilterExpressionsBuilder(
        ISearchFilterFactory<TProjection> filterExpressionFactory,
        IOptions<FilterKeyToFilterExpressionMapOptions> filterKeyMapOptions)
    {
        ArgumentNullException.ThrowIfNull(filterExpressionFactory);
        ArgumentNullException.ThrowIfNull(filterKeyMapOptions);

        _filterExpressionFactory = filterExpressionFactory;
        _filterKeyMapOptions = filterKeyMapOptions.Value;
    }

    /// <summary>
    /// Builds a composed predicate expression by resolving filter expressions for
    /// each incoming request and delegating composition to the filter expression factory.
    /// </summary>
    public Expression<Func<TProjection, bool>> BuildSearchFilterExpression(
        IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        // Resolve filter names + requests
        ReadOnlyCollection<(string, SearchFilterRequest)> resolved =
            ResolveFilterRequests(searchFilterRequests);

        if (resolved.Count == 0)
        {
            return projection => true;
        }

        // Delegate composition to the factory
        ISpecification<TProjection> combined =
            _filterExpressionFactory.CreateFilter(
                resolved[0].Item1,
                resolved[0].Item2);

        for (int i = 1; i < resolved.Count; i++)
        {
            ISpecification<TProjection> next =
                _filterExpressionFactory.CreateFilter(
                    resolved[i].Item1,
                    resolved[i].Item2);

            // TODO assumes these SearchFilterRequests are always combinatorial AND
            combined = combined.And(next);
        }

        return combined.ToExpression();
    }

    /// <summary>
    /// Resolves filter names and requests for all incoming requests whose keys
    /// are present in the configured filter‑expression map.
    /// </summary>
    private ReadOnlyCollection<(string FilterName, SearchFilterRequest Request)> ResolveFilterRequests(
        IEnumerable<SearchFilterRequest> searchFilterRequests)
    {
        List<(string FilterName, SearchFilterRequest Request)> resolved = [];

        foreach (SearchFilterRequest request in searchFilterRequests)
        {
            if (!_filterKeyMapOptions.SearchFilterToExpressionMap
                .TryGetValue(
                    request.FilterKey,
                    out FilterExpressionOptions? options))
            {
                continue;
            }

            if (options.HasValuesDelimiter)
            {
                request.SetFilterValuesDelimiter(
                    options.FilterExpressionValuesDelimiter);
            }

            resolved.Add((options.FilterExpressionKey, request));
        }

        return resolved.AsReadOnly();
    }
}
