using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;

public sealed class DownloadDatasetsResponse
{
    public DownloadDatasetsResponse(Dataset? downloadedDataset)
    {
        DownloadedDataset = downloadedDataset;
    }

    public Dataset? DownloadedDataset { get; }
}
