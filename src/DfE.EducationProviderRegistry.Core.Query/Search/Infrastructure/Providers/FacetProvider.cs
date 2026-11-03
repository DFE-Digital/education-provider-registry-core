using System.Linq.Expressions;
using System.Reflection;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Provides facet aggregation for <see cref="SearchAggregate"/> entities by
/// grouping scalar values or flattening collection values and returning bucket counts.
/// </summary>
public sealed class FacetProvider : IFacetProvider
{
    private readonly FacetOptions _facetOptions;

    /// <summary>
    /// Initialises a new instance of the <see cref="FacetProvider"/> class.
    /// </summary>
    /// <param name="facetOptions">
    /// The configured facet definitions.
    /// </param>
    public FacetProvider(FacetOptions facetOptions)
    {
        ArgumentNullException.ThrowIfNull(facetOptions);

        _facetOptions = facetOptions;
    }

    /// <summary>
    /// Computes facet buckets for the specified facet.
    /// </summary>
    /// <param name="matchingQuery">
    /// The unpaginated query containing the matching search aggregates.
    /// </param>
    /// <param name="facet">
    /// The configured facet name.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token.
    /// </param>
    /// <returns>
    /// The facet buckets ordered by descending count.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the facet is unknown or its configured property path is invalid.
    /// </exception>
    public async Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        string facet,
        IList<object> filterValues,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(matchingQuery);

        if (!_facetOptions.Definitions.TryGetValue(
            facet,
            out FacetDefinition? facetDefinition))
        {
            throw new InvalidOperationException($"Unknown facet '{facet}'.");
        }

        if (string.IsNullOrWhiteSpace(facetDefinition.ValueProperty))
        {
            throw new InvalidOperationException(
                $"Facet '{facet}' does not have a value property defined.");
        }

        PropertyInfo valueProperty =
            ResolveProperty(facetDefinition.ValueProperty);

        return IsCollection(valueProperty.PropertyType)
            ? await GetCollectionFacetsAsync(
                matchingQuery,
                valueProperty,
                [.. filterValues.OfType<string>()],
                cancellationToken)
            : await GetScalarFacetsAsync(
                matchingQuery,
                facetDefinition,
                valueProperty,
                cancellationToken);
    }

    private static async Task<IReadOnlyList<FacetResult>> GetScalarFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        FacetDefinition facetDefinition,
        PropertyInfo valuePropertyInfo,
        CancellationToken cancellationToken)
    {
        ParameterExpression parameter =
            Expression.Parameter(typeof(SearchAggregate), "searchAggregate");

        Expression propertyExpression =
            Expression.Property(parameter, valuePropertyInfo);

        Expression notNullExpression =
            Expression.NotEqual(
                propertyExpression,
                Expression.Constant(null, valuePropertyInfo.PropertyType));

        Expression<Func<SearchAggregate, bool>> notNullPredicate =
            Expression.Lambda<Func<SearchAggregate, bool>>(
                notNullExpression,
                parameter);

        Expression boxValueExpression =
            Expression.Convert(propertyExpression, typeof(object));

        Expression<Func<SearchAggregate, object>> valueSelector =
            Expression.Lambda<Func<SearchAggregate, object>>(
                boxValueExpression,
                parameter);

        IQueryable<IGrouping<object, SearchAggregate>> grouped =
            matchingQuery
                .Where(notNullPredicate)
                .GroupBy(valueSelector);

        if (string.IsNullOrWhiteSpace(facetDefinition.LabelProperty))
        {
            var rawFacetResults =
                await grouped
                    .Select(groupedFacet => new
                    {
                        Value = groupedFacet.Key,
                        Count = groupedFacet.LongCount(),
                    })
                    .ToListAsync(cancellationToken);

            return
            [
                .. rawFacetResults
                .Select(facetResult =>
                {
                    string value =
                        facetResult.Value?.ToString() ?? string.Empty;

                    return new FacetResult(
                        value: value,
                        label: value,
                        count: facetResult.Count);
                })
                .OrderBy(facetResult => facetResult.Label)
            ];
        }

        PropertyInfo labelPropertyInfo =
            ResolveProperty(facetDefinition.LabelProperty);

        if (labelPropertyInfo.PropertyType != typeof(string))
        {
            throw new InvalidOperationException(
                $"Label property '{facetDefinition.LabelProperty}' must be a string property.");
        }

        Expression<Func<SearchAggregate, string?>> labelSelector =
            Expression.Lambda<Func<SearchAggregate, string?>>(
                Expression.Property(parameter, labelPropertyInfo),
                parameter);

        var labelledFacetResults =
            await grouped
                .Select(groupedFacet => new
                {
                    Value = groupedFacet.Key,
                    Label = groupedFacet
                        .AsQueryable()
                        .Select(labelSelector)
                        .FirstOrDefault(),
                    Count = groupedFacet.LongCount(),
                })
                .ToListAsync(cancellationToken);

        return
        [
            .. labelledFacetResults
            .Select(facetResult =>
            {
                string value =
                    facetResult.Value?.ToString() ?? string.Empty;

                return new FacetResult(
                    value: value,
                    label: facetResult.Label ?? value,
                    count: facetResult.Count);
            })
            .OrderByDescending(facetResult => facetResult.Count)
        ];
    }

    private static async Task<IReadOnlyList<FacetResult>> GetCollectionFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        PropertyInfo collectionPropertyInfo,
        string[] filterValues,
        CancellationToken cancellationToken)
    {
        ParameterExpression parameter =
            Expression.Parameter(typeof(SearchAggregate), "searchAggregate");

        Expression<Func<SearchAggregate, IEnumerable<string>>> collectionSelector =
            Expression.Lambda<Func<SearchAggregate, IEnumerable<string>>>(
                Expression.Property(parameter, collectionPropertyInfo),
                parameter);

        IQueryable<string> facetValues =
            matchingQuery
                .SelectMany(collectionSelector)
                .Where(value => !string.IsNullOrEmpty(value));

        // The query filters collections containing a selected value, but collections may contain other values.
        // For example, filtering SEN by "A" may return:
        //   Provider A -> ["A", "B"]
        //   Provider B -> ["A", "C"]
        // Grouping these collections would produce facets for A, B and C, even though only A is selected.
        // Restrict the flattened collection to the selected values before aggregating the facet results.
        // This ensures that only the selected values are returned in the facet results.
        if (filterValues.Length > 0)
        {
            facetValues =
                facetValues.Where(value => filterValues.Contains(value));
        }

        var rawFacetResults =
            await facetValues
                .GroupBy(value => value)
                .Select(groupedFacet => new
                {
                    Value = groupedFacet.Key,
                    Count = groupedFacet.LongCount(),
                })
                .ToListAsync(cancellationToken);

        return
        [
            .. rawFacetResults
            .Select(facetResult =>
                new FacetResult(
                    value: facetResult.Value,
                    label: facetResult.Value,
                    count: facetResult.Count))
            .OrderByDescending(facetResult => facetResult.Count)
        ];
    }

    private static PropertyInfo ResolveProperty(string propertyPath)
    {
        return typeof(SearchAggregate).GetProperty(propertyPath)
            ?? throw new InvalidOperationException(
                $"Property path '{propertyPath}' does not resolve to a valid property.");
    }

    private static bool IsCollection(Type propertyType)
    {
        return propertyType != typeof(string) &&
            typeof(IEnumerable<string>).IsAssignableFrom(propertyType);
    }
}
