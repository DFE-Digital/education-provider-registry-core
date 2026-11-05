using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;

public sealed class DownloadDatasetsRequest :
    IUseCaseRequest<UseCaseResponse<DownloadDatasetsResponse>>
{
    public DownloadDatasetsRequest(string datasetName, string[] fileNameKeys)
    {
        DatasetName = datasetName ??
            throw new ArgumentNullException(nameof(datasetName));
        FileNameKeys = fileNameKeys ??
            throw new ArgumentNullException(nameof(fileNameKeys));
    }

    public string DatasetName { get; }
    public string[] FileNameKeys { get; }
}
