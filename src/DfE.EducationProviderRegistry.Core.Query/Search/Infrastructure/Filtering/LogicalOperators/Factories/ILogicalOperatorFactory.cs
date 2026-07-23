namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;

/// <summary>
/// Resolves logical operators used to combine filter expressions for
/// <typeparamref name="TProjection"/>. Implementations map operator names
/// (e.g., AND, OR, NOT) to typed logical‑operator combinators that merge
/// expression‑tree predicates into a single filter expression.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type for which logical operators are resolved.
/// </typeparam>
public interface ILogicalOperatorFactory<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Retrieves a logical operator combinator by name.
    /// </summary>
    /// <param name="logicalOperatorName">
    /// The operator name to resolve (e.g., AND, OR, NOT).
    /// </param>
    /// <returns>
    /// An <see cref="ILogicalOperator{TProjection}"/> capable of combining
    /// two predicate expressions.
    /// </returns>
    ILogicalOperator<TProjection> Resolve(string logicalOperatorName);
}
