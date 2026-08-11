namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;

/// <summary>
/// Provides a type‑keyed storage mechanism for passing state between
/// search pipeline steps.
/// </summary>
public sealed class SearchPipelineContext
{
    private readonly Dictionary<Type, object> _items = [];

    /// <summary>
    /// Stores a value in the context under its concrete type.
    /// </summary>
    /// <typeparam name="TContextState">The type used as the key.</typeparam>
    /// <param name="value">The value to store.</param>
    public void Set<TContextState>(TContextState value)
    {
        _items[typeof(TContextState)] = value!;
    }

    /// <summary>
    /// Retrieves a value previously stored under the specified type.
    /// </summary>
    /// <typeparam name="TContextState">The expected value type.</typeparam>
    /// <returns>The stored value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no value exists for the requested type.
    /// </exception>
    public TContextState Get<TContextState>()
    {
        if (_items.TryGetValue(typeof(TContextState), out object? value))
        {
            return (TContextState)value;
        }

        throw new InvalidOperationException(
            "PipelineContext does not contain a value of type " + typeof(TContextState).Name);
    }

    /// <summary>
    /// Attempts to retrieve a value stored under the specified type.
    /// </summary>
    /// <typeparam name="TContextState">The expected value type.</typeparam>
    /// <param name="value">The retrieved value, or default if not found.</param>
    /// <returns><c>true</c> if the value exists; otherwise <c>false</c>.</returns>
    public bool TryGet<TContextState>(out TContextState value)
    {
        if (_items.TryGetValue(typeof(TContextState), out object? obj))
        {
            value = (TContextState)obj;
            return true;
        }

        value = default!;
        return false;
    }
}
