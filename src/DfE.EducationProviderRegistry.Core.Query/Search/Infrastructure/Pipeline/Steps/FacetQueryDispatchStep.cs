using System.Collections.ObjectModel;
using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryDispatchStep : BaseEvaluationHandler<SearchPipelineContext>
{
    private readonly IFacetProvider _facetProvider;

    public FacetQueryDispatchStep(
        IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    public override bool CanHandle(SearchPipelineContext request) => true;

    protected override ValueTask HandleCoreAsync(SearchPipelineContext request, CancellationToken cancellationToken = default)
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
