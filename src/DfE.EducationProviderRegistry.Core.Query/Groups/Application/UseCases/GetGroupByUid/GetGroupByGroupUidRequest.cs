using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;

public sealed record GetGroupByGroupUidRequest(string GroupUid) : IUseCaseRequest<UseCaseResponse<GroupReadModelResponse>>;
