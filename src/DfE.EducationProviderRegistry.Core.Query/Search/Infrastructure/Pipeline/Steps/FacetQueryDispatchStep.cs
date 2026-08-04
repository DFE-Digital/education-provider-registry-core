using System.Collections.ObjectModel;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryDispatchStep : IEvaluationHandler<SearchPipelineContext>
{
    private readonly IFacetProvider _facetProvider;

    /// <summary>
    /// Creates a new instance of the dispatch step.
    /// </summary>
    public FacetQueryDispatchStep(IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    public ValueTask HandleAsync(SearchPipelineContext request, CancellationToken cancellationToken = default)
    {
        ReadOnlyCollection<string> ids =
            request.Get<ReadOnlyCollection<string>>()
                ?? throw new InvalidOperationException(
                    "PipelineContext does not contain establishment IDs.");

        List<string> facetNames =
            request.Get<List<string>>()
                ?? throw new InvalidOperationException(
                    "PipelineContext does not contain facet names.");

        if (facetNames.Count == 0)
        {
            request.Set(new List<(
                string FacetName, Task<IReadOnlyList<FacetResult>> Task)>());

            return ValueTask.CompletedTask;
        }

        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            new(facetNames.Count);

        foreach (string facetName in facetNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(facetName))
            {
                throw new InvalidOperationException(
                    "Facet name cannot be null or empty.");
            }

            Task<IReadOnlyList<FacetResult>> task =
                _facetProvider.GetFacetsAsync(ids, facetName, cancellationToken);

            tasks.Add((facetName, task));
        }

        request.Set(tasks);

        return ValueTask.CompletedTask;
    }
}
