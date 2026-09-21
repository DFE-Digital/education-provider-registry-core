using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.UseCases.Request;

public sealed class DownloadDatasetsRequest :
    IUseCaseRequest<UseCaseResponse<DownloadDatasetsResponse>>
{
    public DownloadDatasetsRequest(string filename)
    {
        Filename = filename;
    }

    public string Filename { get; }
}
