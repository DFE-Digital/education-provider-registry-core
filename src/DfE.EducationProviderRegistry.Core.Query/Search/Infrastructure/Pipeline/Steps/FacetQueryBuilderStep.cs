using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryBuilderStep : ISearchPipelineStep
{
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
