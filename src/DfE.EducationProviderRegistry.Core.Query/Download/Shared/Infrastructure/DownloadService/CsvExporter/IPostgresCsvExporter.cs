namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.CsvExporter;

public interface IPostgresCsvExporter
{
    Task ExportAsync(
        string outputPath,
        string query,
        CancellationToken cancellationToken);
}
