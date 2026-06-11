using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups;

public sealed record GetGroupByGroupIdRequest(string GroupId) :
    IUseCaseRequest<UseCaseResponse<GroupDto>>;
