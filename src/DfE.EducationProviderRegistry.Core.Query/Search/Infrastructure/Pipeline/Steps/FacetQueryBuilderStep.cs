using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Builds <see cref="SearchFacet"/> results from completed facet tasks stored
/// in the <see cref="SearchPipelineContext"/>.
/// </summary>
internal sealed class FacetQueryBuilderStep : ISearchPipelineStep
{
    /// <summary>
    /// Converts completed facet tasks into <see cref="SearchFacet"/> instances
    /// and stores them back into the pipeline context.
    /// </summary>
    /// <param name="context">The pipeline context containing facet tasks.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a facet task is incomplete, faulted, or returns null results.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

        List<SearchFacet> facets = new(tasks.Count);

        foreach ((string facetName, Task<IReadOnlyList<FacetResult>> task) in tasks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!task.IsCompleted)
            {
                throw new InvalidOperationException(
                    $"Facet task for '{facetName}' was not completed before FacetQueryBuilderStep.");
            }

            if (task.IsFaulted)
            {
                Exception inner = task.Exception?.InnerException ?? task.Exception!;
                throw new InvalidOperationException(
                    $"Facet task for '{facetName}' failed.", inner);
            }

            IReadOnlyList<FacetResult> results = task.Result
                ?? throw new InvalidOperationException(
                    $"Facet provider returned null results for facet '{facetName}'.");

            facets.Add(new SearchFacet(facetName, [.. results]));
        }

        context.Set(facets);
    }
}
