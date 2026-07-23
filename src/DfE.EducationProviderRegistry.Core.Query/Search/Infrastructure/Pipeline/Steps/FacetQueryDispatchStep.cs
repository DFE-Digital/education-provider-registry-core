using System.Collections.ObjectModel;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Dispatches facet queries by invoking the configured <see cref="IFacetProvider"/>
/// for each facet name and storing the resulting tasks in the pipeline context.
/// </summary>
internal sealed class FacetQueryDispatchStep : ISearchPipelineStep
{
    private readonly IFacetProvider _facetProvider;

    /// <summary>
    /// Creates a new instance of the dispatch step.
    /// </summary>
    public FacetQueryDispatchStep(IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    /// <summary>
    /// Starts facet‑provider tasks for each facet name and stores them in the
    /// <see cref="SearchPipelineContext"/> for later processing.
    /// </summary>
    /// <param name="context">The pipeline context containing IDs and facet names.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required context values are missing or invalid.
    /// </exception>
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
            context.Set(new List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>());
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
