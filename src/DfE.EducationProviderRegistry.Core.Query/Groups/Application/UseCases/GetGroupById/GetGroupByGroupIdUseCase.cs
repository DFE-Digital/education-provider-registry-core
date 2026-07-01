using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

internal sealed class GetGroupByGroupIdUseCase : IUseCase<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModel>>
{
    private readonly ILogger<GetGroupByGroupIdUseCase> _logger;
    private readonly IGroupsRepository _groupsRepository;
    private readonly IMapper<Group, GroupReadModel> _modelToDtoMapper;

    public GetGroupByGroupIdUseCase(
        ILogger<GetGroupByGroupIdUseCase> logger,
        IGroupsRepository groupsRepository,
        IMapper<Group, GroupReadModel> modelToDtoMapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(groupsRepository);
        ArgumentNullException.ThrowIfNull(modelToDtoMapper);

        _logger = logger;
        _groupsRepository = groupsRepository;
        _modelToDtoMapper = modelToDtoMapper;
    }

    public async Task<UseCaseResponse<GroupReadModel>> HandleRequestAsync(
        GetGroupByGroupUniqueIdentifierRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return UseCaseResponse<GroupReadModel>.Failure("The request cannot be null.");
        }

        try
        {
            if (!GroupUID.TryCreate(request.GroupUid, out GroupUID groupUid))
            {
                return UseCaseResponse<GroupReadModel>.Failure($"Could not parse the GroupUniqueIdentifier {request.GroupUid}");
            }

            Group? group = await _groupsRepository.GetGroupByGroupUidAsync(groupUid, cancellationToken);

            if (group is null)
            {
                return UseCaseResponse<GroupReadModel>.Failure(
                    $"Group with GroupId {request.GroupUid} not found.");
            }

            GroupReadModel dto = _modelToDtoMapper.Map(group);

            return UseCaseResponse<GroupReadModel>.Success(dto);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetGroupByGroupIdUseCase));

            return UseCaseResponse<GroupReadModel>.Failure("The operation was cancelled.");
        }
        catch (InvalidGroupIdentifierException ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} validation failed for GroupId {GroupId}",
                nameof(GetGroupByGroupIdUseCase),
                request.GroupUid);

            return UseCaseResponse<GroupReadModel>.Failure("Invalid group identifier.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} execution failed unexpectedly.",
                nameof(GetEstablishmentsUseCase));

            return UseCaseResponse<GroupReadModel>.Failure("An unexpected error occurred.");
        }
    }
}
