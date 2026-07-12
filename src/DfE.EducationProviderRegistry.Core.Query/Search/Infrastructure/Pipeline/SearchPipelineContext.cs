namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;

public sealed class SearchPipelineContext
{
    private readonly Dictionary<Type, object> _items = [];

    public void Set<TContextState>(TContextState value)
    {
        _items[typeof(TContextState)] = value!;
    }

    public TContextState Get<TContextState>()
    {
        if (_items.TryGetValue(typeof(TContextState), out object? value))
        {
            return (TContextState)value;
        }

        throw new InvalidOperationException(
            "PipelineContext does not contain a value of type " + typeof(TContextState).Name);
    }

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
