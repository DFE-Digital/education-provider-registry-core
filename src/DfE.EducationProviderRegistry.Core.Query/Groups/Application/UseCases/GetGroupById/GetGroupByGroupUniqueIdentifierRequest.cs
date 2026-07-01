using DfE.Core.Libraries.CleanArchitecture.Application;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

public sealed record GetGroupByGroupUniqueIdentifierRequest(string GroupUid) :
    IUseCaseRequest<UseCaseResponse<GroupReadModel>>;
