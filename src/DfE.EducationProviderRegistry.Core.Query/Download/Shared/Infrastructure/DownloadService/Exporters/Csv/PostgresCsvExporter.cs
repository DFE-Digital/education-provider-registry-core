namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvExporter : IPostgresCsvExporter
{
    private readonly IPostgresCsvReader _reader;

    public PostgresCsvExporter(
        IPostgresCsvReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        _reader = reader;
    }

    public Task ExportToStreamAsync(
        Stream targetStream,
        string query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(targetStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        return _reader.CopyToStreamAsync(
            targetStream,
            query,
            cancellationToken);
    }
}
