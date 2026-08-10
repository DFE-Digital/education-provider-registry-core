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

    public SearchFilterExpressionFactory(
        Dictionary<string, Func<ISearchFilterExpression>> filterExpressionFactory)
    {
        _filterExpressionFactory = filterExpressionFactory;
    }

    public ISearchFilterExpression CreateFilter<TSearchFilterExpression>()
        where TSearchFilterExpression : ISearchFilterExpression =>
        CreateFilter(typeof(TSearchFilterExpression));

    public ISearchFilterExpression CreateFilter(Type filterType) =>
        CreateFilter(filterName: filterType.Name);

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
