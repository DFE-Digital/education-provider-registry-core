using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.Infrastructure;

public interface IDatasetDownloadRepository
{
    Task<Dataset?> GetDatasetByName(
        string datasetName,
        CancellationToken cancellationToken = default);
}
