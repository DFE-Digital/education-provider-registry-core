using System.Collections.ObjectModel;
using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

internal sealed class SearchOrderMapStep : BaseEvaluationHandler<SearchPipelineContext>
{
    public override bool CanHandle(SearchPipelineContext request) => true;
    protected override ValueTask HandleCoreAsync(SearchPipelineContext request, CancellationToken cancellationToken = default)
    {
        ReadOnlyCollection<string> ids =
         request.Get<ReadOnlyCollection<string>>()
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

        request.Set(orderMap);

        return ValueTask.CompletedTask;
    }
}
