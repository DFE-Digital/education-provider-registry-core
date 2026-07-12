using System;
using System.Collections.Generic;
using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;

/// <summary>
/// Factory responsible for creating concrete <see cref="ISearchFilterExpression"/> instances.
///
/// This factory is configured via dependency injection. The container supplies a dictionary
/// mapping filter expression names to delegates that construct the corresponding
/// <see cref="ISearchFilterExpression"/>. Each delegate resolves the expression from a DI
/// scope, ensuring correct lifetime management.
///
/// Typical registration:
/// <code>
/// services.TryAddSingleton&lt;ISearchFilterExpressionFactory&gt;(provider =>
/// {
///     using var scope = provider.CreateScope();
///
///     var expressions = new Dictionary&lt;string, Func&lt;ISearchFilterExpression&gt;&gt;
///     {
///         ["SearchInFilterExpression"] = () =>
///             scope.ServiceProvider.GetRequiredService&lt;SearchInFilterExpression&gt;(),
///
///         ["LessThanOrEqualToExpression"] = () =>
///             scope.ServiceProvider.GetRequiredService&lt;LessThanOrEqualToExpression&gt;(),
///
///         ["SearchGeoLocationFilterExpression"] = () =>
///             scope.ServiceProvider.GetRequiredService&lt;SearchGeoLocationFilterExpression&gt;()
///     };
///
///     return new SearchFilterExpressionFactory(expressions);
/// });
/// </code>
/// </summary>
internal sealed class SearchFilterExpressionFactory : ISearchFilterExpressionFactory
{
    private readonly Dictionary<string, Func<ISearchFilterExpression>> _filterExpressionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchFilterExpressionFactory"/> class.
    /// The factory uses a dictionary of delegates injected via the IOC container, allowing
    /// object lifetime and scope to be managed at the composition root.
    /// </summary>
    /// <param name="filterExpressionFactory">
    /// A dictionary mapping filter names to delegates that construct the requested
    /// <see cref="ISearchFilterExpression"/> implementation.
    /// </param>
    public SearchFilterExpressionFactory(
        Dictionary<string, Func<ISearchFilterExpression>> filterExpressionFactory)
    {
        _filterExpressionFactory = filterExpressionFactory;
    }

    /// <summary>
    /// Creates an <see cref="ISearchFilterExpression"/> instance based on the generic type specified.
    /// </summary>
    /// <typeparam name="TSearchFilterExpression">
    /// The concrete type of <see cref="ISearchFilterExpression"/> requested.
    /// </typeparam>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    public ISearchFilterExpression CreateFilter<TSearchFilterExpression>()
        where TSearchFilterExpression : ISearchFilterExpression =>
        CreateFilter(typeof(TSearchFilterExpression));

    /// <summary>
    /// Creates an <see cref="ISearchFilterExpression"/> instance based on the type requested.
    /// </summary>
    /// <param name="filterType">
    /// The concrete implementation type of <see cref="ISearchFilterExpression"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    public ISearchFilterExpression CreateFilter(Type filterType) =>
        CreateFilter(filterName: filterType.Name);

    /// <summary>
    /// Creates an <see cref="ISearchFilterExpression"/> instance based on the type name requested.
    /// </summary>
    /// <param name="filterName">
    /// The name of the concrete implementation type of <see cref="ISearchFilterExpression"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if an invalid filter name string is provided.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if a request is made to derive an unknown
    /// <see cref="ISearchFilterExpression"/> instance from the underlying dictionary.
    /// </exception>
    public ISearchFilterExpression CreateFilter(string filterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filterName);

        if (!_filterExpressionFactory.TryGetValue(filterName, out Func<ISearchFilterExpression>? factory) ||
            factory is null)
        {
            throw new ArgumentOutOfRangeException(
                $"Search expression filter of type {filterName} is not registered.");
        }

        return factory();
    }
}
