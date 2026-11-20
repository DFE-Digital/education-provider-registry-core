using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;

public sealed class DownloadDatasetsResponse
{
    public DownloadDatasetsResponse(DatasetFile? downloadedDataset)
    {
        DownloadedDataset = downloadedDataset;
    }

    public DatasetFile? DownloadedDataset { get; }
}
