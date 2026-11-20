namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public interface IPostgresCsvReader
{
    Task CopyToStreamAsync(
        Stream targetStream,
        IQueryable<object> query,
        CancellationToken cancellationToken);
}
