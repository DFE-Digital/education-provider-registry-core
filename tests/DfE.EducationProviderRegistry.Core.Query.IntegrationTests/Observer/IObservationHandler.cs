namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor;

public interface IObservationHandler<TObservation> : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync();
    Task<TObservation> GetObservationsAsync(CancellationToken ct = default);
}
