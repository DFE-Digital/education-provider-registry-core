using System.IO.Compression;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Infrastructure;

public sealed class DatasetDownloadRepository : IDatasetDownloadRepository
{
    private readonly IDownloadExporter _postgresCsvReader;
    private readonly IQueryEngine _queryEngine;

    public DatasetDownloadRepository(
        IDownloadExporter postgresCsvReader,
        IQueryEngine queryEngine)
    {
        ArgumentNullException.ThrowIfNull(postgresCsvReader);
        ArgumentNullException.ThrowIfNull(queryEngine);

        _postgresCsvReader = postgresCsvReader;
        _queryEngine = queryEngine;
    }

    public async Task<DatasetFile?> DownloadDatasetByNameAsync(
        string datasetName,
        string[] fileNameKeys,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(datasetName);
        ArgumentNullException.ThrowIfNull(fileNameKeys);

        if (fileNameKeys.Length == 0)
        {
            return null;
        }

        DateTime downloadDate = DateTime.Now;

        MemoryStream memoryStream = new();

        try
        {
            using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (string fileKey in fileNameKeys)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string entryName =
                        $"{fileKey}_{downloadDate:yyyyMMdd}.csv";

                    ZipArchiveEntry entry =
                        archive.CreateEntry(
                            entryName,
                            CompressionLevel.Optimal);

                    await using Stream entryStream = await entry.OpenAsync(cancellationToken);

                    IQueryable<object> queryable =
                        _queryEngine.BuildQuery(fileKey);

                    ArgumentNullException.ThrowIfNull(queryable);

                    await _postgresCsvReader.ExportToStreamAsync(
                        entryStream,
                        queryable,
                        cancellationToken);
                }
            }

            memoryStream.Position = 0;

            return new DatasetFile(
                Filename: $"{datasetName}_{downloadDate:yyyyMMdd}.zip",
                DataType: datasetName,
                DataFormat: "ZIP",
                FileSize: memoryStream.Length,
                FileStream: memoryStream);
        }
        catch
        {
            await memoryStream.DisposeAsync();
            throw;
        }
    }
}
