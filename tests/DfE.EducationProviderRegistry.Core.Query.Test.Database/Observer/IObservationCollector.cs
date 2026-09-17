namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Observer;

public interface IObservationCollector<TObservation> : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync();
    Task<TObservation> GetObservationsAsync(CancellationToken ct = default);
}
