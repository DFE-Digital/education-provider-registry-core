using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;

namespace DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;

internal sealed class PipelineEvaluator : IEvaluator<SearchPipelineContext>
{
    private readonly IReadOnlyList<IEvaluationHandler<SearchPipelineContext>> _handlers;

    public PipelineEvaluator(IEnumerable<IEvaluationHandler<SearchPipelineContext>> handlers)
    {
        _handlers = handlers?.ToList() ?? throw new ArgumentNullException(nameof(handlers));

        if (_handlers.Count == 0)
        {
            throw new ArgumentException("No handlers registered");
        }
    }

    public async ValueTask EvaluateAsync(SearchPipelineContext request, CancellationToken ct = default)
    {
        foreach (IEvaluationHandler<SearchPipelineContext> handler in _handlers)
        {
            await handler.HandleAsync(request, ct);
        }
    }
}
