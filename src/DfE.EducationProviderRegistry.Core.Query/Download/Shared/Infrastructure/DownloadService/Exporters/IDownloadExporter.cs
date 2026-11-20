namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters;

public interface IDownloadExporter
{
    Task ExportToStreamAsync(
        Stream targetStream,
        IQueryable<object> query,
        CancellationToken cancellationToken);
}
