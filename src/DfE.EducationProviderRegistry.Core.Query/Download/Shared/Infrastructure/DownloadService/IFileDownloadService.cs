using DynamicCsvFileGeneration.Datasets;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService;

public interface IFileDownloadService<TInput> where TInput : class
{
    Task QueueAsync<TProjection>(
        DatasetDownloadRequest downloadRequest,
        string fileOutputPath,
        CancellationToken cancellationToken);
}
