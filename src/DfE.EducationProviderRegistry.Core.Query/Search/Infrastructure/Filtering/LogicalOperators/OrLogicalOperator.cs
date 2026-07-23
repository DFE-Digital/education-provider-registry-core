using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;

/// <summary>
/// Represents a logical operator that merges two predicate expressions for
/// <typeparamref name="TProjection"/> using Boolean OR semantics.
/// <para>
/// Both input expressions are invoked against a shared parameter, ensuring the
/// resulting expression tree is unified, provider‑agnostic, and suitable for
/// translation by the search pipeline (e.g., EF Core, SQL translators).
/// </para>
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type the operator applies to.
/// </typeparam>
public sealed class OrLogicalOperator<TProjection> : ILogicalOperator<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Combines two predicate expressions into a single Boolean expression
    /// representing <c>left OR right</c>.
    /// <para>
    /// Each input expression is invoked against a shared parameter to ensure
    /// the composed expression tree is valid and free from mismatched
    /// <see cref="ParameterExpression"/> instances.
    /// </para>
    /// </summary>
    /// <param name="left">The left‑hand predicate expression.</param>
    /// <param name="right">The right‑hand predicate expression.</param>
    /// <returns>
    /// A composed <see cref="Expression{TDelegate}"/> representing the logical
    /// disjunction of <paramref name="left"/> and <paramref name="right"/>.
    /// </returns>
    public Expression<Func<TProjection, bool>> Combine(
        Expression<Func<TProjection, bool>> left,
        Expression<Func<TProjection, bool>> right)
    {
        // Create a single shared parameter to unify both expressions.
        ParameterExpression param =
            Expression.Parameter(typeof(TProjection), "param");

        // Invoke both expressions against the shared parameter.
        Expression body = Expression.OrElse(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param));

        // Build the final unified lambda expression.
        return Expression.Lambda<Func<TProjection, bool>>(body, param);
    }
}
