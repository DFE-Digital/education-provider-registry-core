using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class SearchOrderMapStep : ISearchPipelineStep
{
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
