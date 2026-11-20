using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;

public interface IDatasetDownloadRepository
{
    Task<DatasetFile?> DownloadDatasetByNameAsync(
        string datasetName,
        string[] fileNameKeys,
        CancellationToken cancellationToken = default);
}
