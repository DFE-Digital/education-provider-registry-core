using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class ParallelMappingStep : ISearchPipelineStep
{
    private readonly IMapper<Establishment, EstablishmentSearchResult> _mapper;

    public ParallelMappingStep(
        IMapper<Establishment, EstablishmentSearchResult> mapper)
    {
        _mapper = mapper;
    }

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

                EstablishmentSearchResult mapped =
                    _mapper.Map(establishment);

                results[index] = mapped;
            });

        context.Set(results);
    }
}
