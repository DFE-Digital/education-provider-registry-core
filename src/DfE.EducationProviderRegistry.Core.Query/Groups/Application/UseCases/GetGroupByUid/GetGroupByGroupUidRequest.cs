using DfE.Core.Libraries.CleanArchitecture.Application;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;

public sealed record GetGroupByGroupUidRequest(string GroupUid) : IUseCaseRequest<UseCaseResponse<GroupReadModelResponse>>;
