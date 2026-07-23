using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryBuilderStep : BaseEvaluationHandler<SearchPipelineContext>
{
    public override bool CanHandle(SearchPipelineContext request) => true;

    protected override ValueTask HandleCoreAsync(SearchPipelineContext request, CancellationToken cancellationToken = default)
    {
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            request.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

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

        request.Set(facets);

        return ValueTask.CompletedTask;
    }
}
