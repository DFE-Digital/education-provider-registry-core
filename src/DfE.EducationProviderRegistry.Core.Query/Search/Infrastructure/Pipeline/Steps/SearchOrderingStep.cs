using DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class SearchOrderingStep : IEvaluationHandler<SearchPipelineContext>
{
    public ValueTask HandleAsync(SearchPipelineContext request, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Establishment> establishments =
            request.Get<IReadOnlyList<Establishment>>()
                ?? throw new InvalidOperationException(
                    "PipelineContext does not contain establishments to order.");

        Dictionary<string, int> orderMap =
            request.Get<Dictionary<string, int>>()
                ?? throw new InvalidOperationException(
                    "PipelineContext does not contain an order map.");

        List<Establishment> ordered = new(establishments.Count);

        foreach (Establishment establishment in establishments)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string urn = establishment.Urn
                ?? throw new InvalidOperationException(
                    "Establishment has a null URN and cannot be ordered.");

            if (!orderMap.TryGetValue(urn, out int _))
            {
                throw new InvalidOperationException(
                    $"Order map does not contain an entry for URN '{urn}'.");
            }

            ordered.Add(establishment);
        }

        ordered.Sort((left, right) =>
            orderMap[left.Urn!].CompareTo(orderMap[right.Urn!]));

        request.Set(ordered);

        return ValueTask.CompletedTask;
    }
}
