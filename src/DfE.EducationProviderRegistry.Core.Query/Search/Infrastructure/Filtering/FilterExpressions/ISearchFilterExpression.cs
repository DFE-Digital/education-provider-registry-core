using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

/// <summary>
/// Defines a typed filter expression for <typeparamref name="TProjection"/>.
/// Implementations construct provider‑agnostic expression trees based on the
/// supplied <see cref="SearchFilterRequest"/>.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type for which the filter expression is generated.
/// </typeparam>
public interface ISearchFilterExpression<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Produces a typed predicate expression for <typeparamref name="TProjection"/>
    /// based on the supplied filter request.
    /// </summary>
    /// <param name="request">
    /// The filter key and values used to construct the predicate expression.
    /// </param>
    /// <returns>
    /// A provider‑agnostic <see cref="Expression{TDelegate}"/> describing the
    /// filter logic for <typeparamref name="TProjection"/>.
    /// </returns>
    Expression<Func<TProjection, bool>> ToExpression(SearchFilterRequest request);
}
