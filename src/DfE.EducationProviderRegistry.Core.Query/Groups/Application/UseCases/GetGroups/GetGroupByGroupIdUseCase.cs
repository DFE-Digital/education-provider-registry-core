using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups;

internal sealed class GetGroupByGroupIdUseCase : IUseCase<GetGroupByGroupIdRequest, UseCaseResponse<GroupDto>>
{
    private readonly IGroupsRepository _groupsRepository;
    private readonly IMapper<Group, GroupDto> _modelToDtoMapper;

    public GetGroupByGroupIdUseCase(
        IGroupsRepository groupsRepository,
        IMapper<Group, GroupDto> modelToDtoMapper)
    {
        ArgumentNullException.ThrowIfNull(groupsRepository);
        ArgumentNullException.ThrowIfNull(modelToDtoMapper);
        _groupsRepository = groupsRepository;
        _modelToDtoMapper = modelToDtoMapper;
    }

    public async Task<UseCaseResponse<GroupDto>> HandleRequestAsync(GetGroupByGroupIdRequest request, CancellationToken cancellationToken = default)
    {
        GroupIdentifier identifier = new(request.GroupId);

        Group? group = await _groupsRepository.GetGroupByGroupIdAsync(identifier, cancellationToken);

        if (group is null)
        {
            return UseCaseResponse<GroupDto>.Failure($"Group with GroupId {request.GroupId} not found.");
        }
        try
        {
            GroupDto dto = _modelToDtoMapper.Map(group);
            return UseCaseResponse<GroupDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return UseCaseResponse<GroupDto>.Failure($"An error occurred while mapping the group to a DTO: {ex.Message}");
        }
    }
}
