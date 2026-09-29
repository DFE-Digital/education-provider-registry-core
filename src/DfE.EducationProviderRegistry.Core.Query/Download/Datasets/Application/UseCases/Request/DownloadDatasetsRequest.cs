using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;

public sealed class DownloadDatasetsRequest :
    IUseCaseRequest<UseCaseResponse<DownloadDatasetsResponse>>
{
    public DownloadDatasetsRequest(string filename)
    {
        Filename = filename ??
            throw new ArgumentNullException(nameof(filename));
    }

    public string Filename { get; }
}
