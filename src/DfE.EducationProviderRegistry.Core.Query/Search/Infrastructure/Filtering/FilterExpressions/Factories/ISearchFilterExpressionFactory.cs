namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;

public interface ISearchFilterExpressionFactory
{
    /// <summary>
    /// Allows creation of an <see cref="ISearchFilterExpression"/> instance based on the type requested.
    /// </summary>
    /// <param name="filterType">
    /// The concrete implementation type of <see cref="ISearchFilterExpression"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    ISearchFilterExpression CreateFilter(Type filterType);

    /// <summary>
    /// Allows creation of an <see cref="ISearchFilterExpression"/> instance based on the type name requested.
    /// </summary>
    /// <param name="filterName">
    /// The name of the concrete implementation type <see cref="ISearchFilterExpression"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    ISearchFilterExpression CreateFilter(string filterName);

    /// <summary>
    /// Allows creation of an <see cref="ISearchFilterExpression"/> instance based on the generic type specified.
    /// </summary>
    /// <typeparam name="TSearchFilterExpression">
    /// The concrete type of <see cref="ISearchFilterExpression"/> requested.
    /// </typeparam>
    /// <returns>
    /// The configured instance of the <see cref="ISearchFilterExpression"/> type.
    /// </returns>
    ISearchFilterExpression CreateFilter<TSearchFilterExpression>() where TSearchFilterExpression : ISearchFilterExpression;
}
