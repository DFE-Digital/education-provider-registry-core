using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Resolves facet query tasks previously dispatched in the pipeline by awaiting
/// their completion.
/// </summary>
internal sealed class FacetQueryResolverStep : ISearchPipelineStep
{
    /// <summary>
    /// Waits for all facet tasks stored in the <see cref="SearchPipelineContext"/>
    /// to complete, propagating any underlying task failures.
    /// </summary>
    /// <param name="context">The pipeline context containing facet tasks.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required tasks are missing or when one or more facet tasks fail.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>()
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
    }
}
