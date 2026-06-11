using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

public sealed record GetGroupByGroupIdRequest(string GroupId) :
    IUseCaseRequest<UseCaseResponse<GroupDto>>;
