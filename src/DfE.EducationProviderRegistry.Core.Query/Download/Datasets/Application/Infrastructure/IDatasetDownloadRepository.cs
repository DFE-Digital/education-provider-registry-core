using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;

public interface IDatasetDownloadRepository
{
    Task<Dataset?> GetDatasetByName(
        string datasetName,
        CancellationToken cancellationToken = default);
}
