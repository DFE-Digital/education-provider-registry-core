namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public interface IPostgresCsvExporter
{
    Task ExportToStreamAsync(Stream targetStream, string query, CancellationToken cancellationToken);
}
