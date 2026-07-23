using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Orders <see cref="Establishment"/> entities according to the order map stored
/// in the <see cref="SearchPipelineContext"/>.
/// </summary>
internal sealed class SearchOrderingStep : ISearchPipelineStep
{
    /// <summary>
    /// Applies the configured order map to the establishments in the pipeline
    /// context and stores the ordered list back into the context.
    /// </summary>
    /// <param name="context">The pipeline context containing establishments and an order map.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required context entries are missing or when an establishment
    /// URN is absent from the order map.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        IReadOnlyList<Establishment> establishments =
            context.Get<IReadOnlyList<Establishment>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain establishments to order.");

        Dictionary<string, int> orderMap =
            context.Get<Dictionary<string, int>>()
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

        context.Set(ordered);
    }
}
