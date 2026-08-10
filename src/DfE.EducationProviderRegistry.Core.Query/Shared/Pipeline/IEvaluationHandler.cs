namespace DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;

public interface IEvaluationHandler<in TRequest>
{
    ValueTask HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default);
}
