using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Application.UseCases.Request;

public sealed class DownloadRequest : IUseCaseRequest<UseCaseResponse<DownloadResponse>>
{
}
