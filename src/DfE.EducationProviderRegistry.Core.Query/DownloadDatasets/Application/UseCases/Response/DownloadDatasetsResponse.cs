using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Response;

public sealed class DownloadDatasetsResponse
{
    public DownloadDatasetsResponse(Dataset? downloadedDataset)
    {
        DownloadedDataset = downloadedDataset;
    }

    public Dataset? DownloadedDataset { get; }
}
