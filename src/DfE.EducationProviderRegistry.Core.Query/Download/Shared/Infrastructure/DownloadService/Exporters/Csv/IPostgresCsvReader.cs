namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public interface IPostgresCsvReader : IAsyncDisposable
{
    Task OpenAsync(
        string query,
        CancellationToken cancellationToken);

    ValueTask<int> ReadAsync(
        Memory<char> buffer,
        CancellationToken cancellationToken);
}
