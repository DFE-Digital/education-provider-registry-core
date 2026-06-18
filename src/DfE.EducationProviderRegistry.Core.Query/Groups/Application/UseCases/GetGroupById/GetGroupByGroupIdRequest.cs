using DfE.Core.Libraries.CleanArchitecture.Application;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

public sealed record GetGroupByGroupIdRequest(string GroupId) :
    IUseCaseRequest<UseCaseResponse<GroupReadModel>>;
