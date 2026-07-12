namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;

public interface ISearchPipelineStep
{
    /// <summary>
    /// Executes the pipeline step using the provided <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The shared <see cref="SearchPipelineContext"/> containing all state
    /// accumulated so far in the pipeline. Implementations may read from or
    /// write to this context to contribute to the overall search result.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to observe cancellation requests. Implementations should
    /// honour this token when performing asynchronous or database‑bound work.
    /// </param>
    void Execute(SearchPipelineContext context, CancellationToken cancellationToken);
}
