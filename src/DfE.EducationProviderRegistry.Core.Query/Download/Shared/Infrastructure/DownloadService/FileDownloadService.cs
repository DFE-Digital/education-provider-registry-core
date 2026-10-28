using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.CsvExporter;
using DynamicCsvFileGeneration.Datasets;
using DynamicCsvFileGeneration.DownloadQueue;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService;

public sealed class FileDownloadService<TInput> : IFileDownloadService<TInput> where TInput : class
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IDatasetDownloadQueryEngine<TInput> _queryEngine;
    private readonly IPostgresCsvExporter _postgresCsvExporter;

    public FileDownloadService(
        IBackgroundTaskQueue taskQueue,
        IDatasetDownloadQueryEngine<TInput> queryEngine,
        IPostgresCsvExporter postgresCsvExporter)
    {
        _taskQueue = taskQueue;
        _queryEngine = queryEngine;
        _postgresCsvExporter = postgresCsvExporter;
    }

    public async Task QueueAsync<TProjection>(
        DatasetDownloadRequest downloadRequest,
        string fileOutputPath,
        CancellationToken cancellationToken)
    {
        await _taskQueue.QueueBackgroundWorkItemAsync(
            async (serviceProvider, token) =>
            {
                IQueryable<TProjection> query =
                    _queryEngine.BuildQuery<TProjection>(downloadRequest);

                await _postgresCsvExporter.ExportAsync(
                    fileOutputPath,
                    query.ToQueryString(),
                    token);
            });
    }
}
