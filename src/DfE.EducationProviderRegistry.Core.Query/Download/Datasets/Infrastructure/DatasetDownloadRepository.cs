using System.IO.Compression;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Infrastructure;

public sealed class DatasetDownloadRepository : IDatasetDownloadRepository
{
    private readonly IPostgresCsvExporter _postgresCsvExporter;
    private readonly IQueryEngine _queryEngine;

    public DatasetDownloadRepository(
        IPostgresCsvExporter postgresCsvExporter,
        IQueryEngine queryEngine)
    {
        _postgresCsvExporter = postgresCsvExporter;
        _queryEngine = queryEngine;
    }

    public async Task<Dataset?> GetDatasetByName(
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

        MemoryStream memoryStream = new();

        using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (string fileKey in fileNameKeys)
            {
                string entryName = $"{fileKey}_{DateTime.Now:yyyyMMdd}.csv";

                ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

                await using Stream entryStream = entry.Open();

                IQueryable<object> queryable =
                    _queryEngine.BuildQuery(fileKey);

                ArgumentNullException.ThrowIfNull(queryable);

                await _postgresCsvExporter
                    .ExportToStreamAsync(entryStream, queryable.ToQueryString(), cancellationToken);
            }
        }

        memoryStream.Position = 0;

        return new Dataset(
            Filename: $"{datasetName}_{DateTime.Now:yyyyMMdd}.zip",
            DataType: datasetName,
            DataFormat: "ZIP",
            FileSize: memoryStream.Length,
            FileStream: memoryStream);
    }
}
