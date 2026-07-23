using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Maps ordered <see cref="Establishment"/> entities to
/// <see cref="EstablishmentSearchResult"/> instances in parallel.
/// </summary>
internal sealed class ParallelMappingStep : ISearchPipelineStep
{
    private readonly IMapper<Establishment, EstablishmentSearchResult> _mapper;

    /// <summary>
    /// Creates a new instance of the mapping step.
    /// </summary>
    public ParallelMappingStep(IMapper<Establishment, EstablishmentSearchResult> mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Performs parallel mapping of ordered establishments and stores the
    /// resulting search results in the pipeline context.
    /// </summary>
    /// <param name="context">The pipeline context containing ordered establishments.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when ordered establishments are missing from the context.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<Establishment> ordered =
            context.Get<IReadOnlyList<Establishment>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain ordered establishments.");

        EstablishmentSearchResult[] results =
            new EstablishmentSearchResult[ordered.Count];

        ParallelOptions options = new()
        {
            CancellationToken = cancellationToken
        };

        Parallel.ForEach(
            Enumerable.Range(0, ordered.Count),
            options,
            index =>
            {
                options.CancellationToken.ThrowIfCancellationRequested();

                Establishment establishment = ordered[index];
                EstablishmentSearchResult mapped = _mapper.Map(establishment);

                results[index] = mapped;
            });

        context.Set(results);
    }
}
