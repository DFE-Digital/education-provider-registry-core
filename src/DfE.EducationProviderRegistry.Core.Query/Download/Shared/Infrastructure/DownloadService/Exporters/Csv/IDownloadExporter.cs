namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public interface IDownloadExporter
{
    Task ExportToStreamAsync(
        Stream targetStream,
        IQueryable<object> query,
        CancellationToken cancellationToken);
}
