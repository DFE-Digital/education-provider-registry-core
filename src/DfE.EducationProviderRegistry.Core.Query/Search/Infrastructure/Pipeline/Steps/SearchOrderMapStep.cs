using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

/// <summary>
/// Builds an order map from establishment URNs stored in the
/// <see cref="SearchPipelineContext"/>.
/// </summary>
internal sealed class SearchOrderMapStep : ISearchPipelineStep
{
    /// <summary>
    /// Creates a dictionary mapping each establishment URN to its positional
    /// index and stores it in the pipeline context.
    /// </summary>
    /// <param name="context">The pipeline context containing establishment URNs.</param>
    /// <param name="cancellationToken">Token used to cancel execution.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when URNs are missing or contain invalid values.
    /// </exception>
    public void Execute(SearchPipelineContext context, CancellationToken cancellationToken)
    {
        ReadOnlyCollection<string> ids =
            context.Get<ReadOnlyCollection<string>>()
            ?? throw new InvalidOperationException(
                "PipelineContext does not contain establishment URNs.");

        Dictionary<string, int> orderMap = new(ids.Count);

        for (int index = 0; index < ids.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string urn = ids[index];

            if (string.IsNullOrWhiteSpace(urn))
            {
                throw new InvalidOperationException(
                    $"Encountered null or empty URN at index {index}.");
            }

            orderMap[urn] = index;
        }

        context.Set(orderMap);
    }
}
