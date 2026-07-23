using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryResolverStep : BaseEvaluationHandler<SearchPipelineContext>
{
    public override bool CanHandle(SearchPipelineContext request) => true;

    protected override ValueTask HandleCoreAsync(SearchPipelineContext request, CancellationToken cancellationToken = default)
    {
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            request.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain facet query tasks.");

        cancellationToken.ThrowIfCancellationRequested();

        Task[] taskArray = new Task[tasks.Count];
        for (int i = 0; i < tasks.Count; i++)
        {
            taskArray[i] = tasks[i].Task;
        }

        try
        {
            Task.WhenAll(taskArray).Wait(cancellationToken);
        }
        catch (AggregateException ex)
        {
            Exception inner = ex.InnerException ?? ex;
            throw new InvalidOperationException(
                "One or more facet tasks failed during resolution.", inner);
        }

        return ValueTask.CompletedTask;
    }
}
