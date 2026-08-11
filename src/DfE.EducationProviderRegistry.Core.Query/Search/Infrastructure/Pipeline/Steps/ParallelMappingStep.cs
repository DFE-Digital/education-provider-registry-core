using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class ParallelMappingStep : IEvaluationHandler<SearchPipelineContext>
{
    private readonly IMapper<Establishment, EstablishmentSearchResult> _mapper;

    /// <summary>
    /// Creates a new instance of the mapping step.
    /// </summary>
    public ParallelMappingStep(IMapper<Establishment, EstablishmentSearchResult> mapper)
    {
        _mapper = mapper;
    }

    public ValueTask HandleAsync(SearchPipelineContext request, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Establishment> ordered =
            request.Get<IReadOnlyList<Establishment>>()
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

        request.Set(results);

        return ValueTask.CompletedTask;
    }
}
