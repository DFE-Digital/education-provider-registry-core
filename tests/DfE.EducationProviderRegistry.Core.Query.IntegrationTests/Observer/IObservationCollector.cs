namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer;

public interface IObservationCollector<TObservation> : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync();
    Task<TObservation> GetObservationsAsync(CancellationToken ct = default);
}
