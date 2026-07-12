using System.Collections.ObjectModel;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class FacetQueryDispatchStep : ISearchPipelineStep
{
    private readonly IFacetProvider _facetProvider;

    public FacetQueryDispatchStep(
        IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        ReadOnlyCollection<string> ids =
            context.Get<ReadOnlyCollection<string>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain establishment IDs.");

        List<string> facetNames =
            context.Get<List<string>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain facet names.");

        if (facetNames.Count == 0)
        {
            context.Set(new List<(
                string FacetName, Task<IReadOnlyList<FacetResult>> Task)>());

            return;
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

        context.Set(tasks);
    }
}
