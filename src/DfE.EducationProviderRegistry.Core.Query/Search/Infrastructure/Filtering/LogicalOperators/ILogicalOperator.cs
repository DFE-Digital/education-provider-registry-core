using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;

/// <summary>
/// Represents a logical operator capable of combining two predicate expressions
/// for <typeparamref name="TProjection"/>. Logical operators merge expression‑tree
/// filters (e.g., AND, OR, NOT) into a single Boolean expression suitable for
/// translation by the search pipeline.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type the operator applies to.
/// </typeparam>
public interface ILogicalOperator<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Combines two predicate expressions into a single Boolean expression using
    /// the operator's logical semantics.
    /// </summary>
    /// <param name="left">The left‑hand predicate expression.</param>
    /// <param name="right">The right‑hand predicate expression.</param>
    /// <returns>
    /// A composed <see cref="Expression{TDelegate}"/> representing the merged
    /// predicate for <typeparamref name="TProjection"/>.
    /// </returns>
    Expression<Func<TProjection, bool>> Combine(
        Expression<Func<TProjection, bool>> left,
        Expression<Func<TProjection, bool>> right);
}
