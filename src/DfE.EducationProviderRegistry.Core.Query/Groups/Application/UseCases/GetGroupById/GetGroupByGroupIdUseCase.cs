using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

internal sealed class GetGroupByGroupIdUseCase : IUseCase<GetGroupByGroupIdRequest, UseCaseResponse<GroupDto>>
{
    private readonly ILogger<GetGroupByGroupIdUseCase> _logger;
    private readonly IGroupsRepository _groupsRepository;
    private readonly IMapper<Group, GroupDto> _modelToDtoMapper;

    public GetGroupByGroupIdUseCase(
        ILogger<GetGroupByGroupIdUseCase> logger,
        IGroupsRepository groupsRepository,
        IMapper<Group, GroupDto> modelToDtoMapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(groupsRepository);
        ArgumentNullException.ThrowIfNull(modelToDtoMapper);

        _logger = logger;
        _groupsRepository = groupsRepository;
        _modelToDtoMapper = modelToDtoMapper;
    }

    public async Task<UseCaseResponse<GroupDto>> HandleRequestAsync(
        GetGroupByGroupIdRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            GroupIdentifier identifier = new(request.GroupId);

            Group? group = await _groupsRepository
                .GetGroupByGroupIdAsync(identifier, cancellationToken);

            if (group is null)
            {
                return UseCaseResponse<GroupDto>.Failure(
                    $"Group with GroupId {request.GroupId} not found.");
            }

            GroupDto dto = _modelToDtoMapper.Map(group);

            return UseCaseResponse<GroupDto>.Success(dto);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetGroupByGroupIdUseCase));

            return UseCaseResponse<GroupDto>.Failure("The operation was cancelled.");
        }
        catch (InvalidGroupIdentifierException ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} validation failed for GroupId {GroupId}",
                nameof(GetGroupByGroupIdUseCase),
                request.GroupId);

            return UseCaseResponse<GroupDto>.Failure("Invalid group identifier.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} execution failed unexpectedly.",
                nameof(GetEstablishmentsUseCase));

            return UseCaseResponse<GroupDto>.Failure("An unexpected error occurred.");
        }
    }
}
