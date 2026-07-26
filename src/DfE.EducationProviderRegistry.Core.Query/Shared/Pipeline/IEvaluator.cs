namespace DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;

public interface IEvaluator<in TRequest>
{
    ValueTask EvaluateAsync(TRequest request, CancellationToken ct = default);
}
