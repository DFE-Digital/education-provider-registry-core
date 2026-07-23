namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;

/// <summary>
/// Resolves logical operators used to combine predicate expressions for
/// <typeparamref name="TProjection"/>. Operators are registered by name and
/// returned as typed combinators capable of merging two Boolean expression trees.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type the logical operators apply to.
/// </typeparam>
public sealed class LogicalOperatorFactory<TProjection> : ILogicalOperatorFactory<TProjection>
    where TProjection : class
{
    private readonly Dictionary<string, Func<ILogicalOperator<TProjection>>> _registry;

    /// <summary>
    /// Creates a new <see cref="LogicalOperatorFactory{TProjection}"/> using the
    /// supplied registry of operator factories.
    /// </summary>
    /// <param name="registry">
    /// A dictionary mapping operator names (e.g., AND, OR, NOT) to delegates that
    /// construct the corresponding <see cref="ILogicalOperator{TProjection}"/>.
    /// </param>
    public LogicalOperatorFactory(
        Dictionary<string, Func<ILogicalOperator<TProjection>>> registry)
    {
        _registry = registry ??
            throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Resolves a logical operator combinator by name.
    /// </summary>
    /// <param name="logicalOperatorName">The operator name to resolve.</param>
    /// <returns>
    /// The configured <see cref="ILogicalOperator{TProjection}"/> instance.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="logicalOperatorName"/> is null or whitespace.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the operator name is not registered.
    /// </exception>
    public ILogicalOperator<TProjection> Resolve(string logicalOperatorName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logicalOperatorName);

        if (!_registry.TryGetValue(logicalOperatorName,
            out Func<ILogicalOperator<TProjection>>? factory) ||
            factory is null)
        {
            throw new ArgumentOutOfRangeException(
                $"Logical operator '{logicalOperatorName}' is not registered.");
        }

        return factory();
    }
}
