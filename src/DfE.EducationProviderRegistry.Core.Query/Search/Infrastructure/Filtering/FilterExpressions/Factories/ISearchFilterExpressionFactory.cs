using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;

/// <summary>
/// Defines the contract for resolving and composing typed filter expressions
/// for <typeparamref name="TProjection"/>. Implementations map filter keys to
/// concrete <see cref="ISearchFilterExpression{TProjection}"/> instances and
/// produce provider‑agnostic predicate expression trees.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type the filter expressions apply to.
/// </typeparam>
public interface ISearchFilterExpressionFactory<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Creates a predicate expression for the filter identified by
    /// <paramref name="filterName"/> using the supplied
    /// <paramref name="request"/>.
    /// </summary>
    /// <param name="filterName">The registered filter key.</param>
    /// <param name="request">The filter request containing values and metadata.</param>
    /// <returns>
    /// A typed predicate expression suitable for composition via logical operators.
    /// </returns>
    Expression<Func<TProjection, bool>> CreateFilter(
        string filterName,
        SearchFilterRequest request);

    /// <summary>
    /// Composes multiple filter expressions using the logical operator identified
    /// by <paramref name="logicalOperatorName"/>.
    /// </summary>
    /// <param name="filters">
    /// A collection of filter keys and their associated requests.
    /// </param>
    /// <param name="logicalOperatorName">
    /// The logical operator used to combine the expressions (e.g., <c>And</c>, <c>Or</c>).
    /// </param>
    /// <returns>
    /// A composed predicate expression representing all filters.
    /// </returns>
    Expression<Func<TProjection, bool>> ComposeFilters(
        IReadOnlyList<(
            string FilterName,
            SearchFilterRequest Request)> filters,
        string logicalOperatorName);

    /// <summary>
    /// Composes already‑instantiated filter objects using the logical operator
    /// identified by <paramref name="logicalOperatorName"/>.
    /// </summary>
    /// <param name="filters">
    /// A collection of filter objects and their associated requests.
    /// </param>
    /// <param name="logicalOperatorName">
    /// The logical operator used to combine the expressions.
    /// </param>
    /// <returns>
    /// A composed predicate expression representing all filters.
    /// </returns>
    Expression<Func<TProjection, bool>> ComposeFilters(
        IReadOnlyList<(
            ISearchFilterExpression<TProjection> Filter,
            SearchFilterRequest Request)> filters,
        string logicalOperatorName);
}
