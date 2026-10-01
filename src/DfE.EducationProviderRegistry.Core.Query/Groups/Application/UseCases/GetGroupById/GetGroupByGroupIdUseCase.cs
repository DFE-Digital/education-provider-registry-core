using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

internal sealed class GetGroupByGroupIdUseCase : IUseCase<GetGroupByGroupUniqueIdentifierRequest, UseCaseResponse<GroupReadModelResponse>>
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

    public async Task<UseCaseResponse<GroupReadModelResponse>> HandleRequestAsync(
        GetGroupByGroupUniqueIdentifierRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return UseCaseResponse<GroupReadModelResponse>.Failure(Failure, "The request cannot be null.");
        }

        try
        {
            if (!GroupUID.TryCreate(request.GroupUid, out GroupUID groupUid))
            {
                return UseCaseResponse<GroupReadModelResponse>.Failure(Failure, $"Could not parse the GroupUniqueIdentifier {request.GroupUid}");
            }

            Group? group = await _groupsRepository.GetGroupByGroupUidAsync(groupUid, cancellationToken);

            if (group is null)
            {
                return UseCaseResponse<GroupReadModelResponse>.Failure(Failure, $"Group with GroupId {request.GroupUid} not found.");
            }

            GroupReadModel dto = _modelToDtoMapper.Map(group);

            return UseCaseResponse<GroupReadModelResponse>.Success(
                new GroupReadModelResponse
                {
                    Group = dto
                });
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetGroupByGroupIdUseCase));

            return UseCaseResponse<GroupReadModelResponse>.Failure(Failure, "The operation was cancelled.");
        }
        catch (InvalidGroupIdentifierException ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} validation failed for GroupId {GroupId}",
                nameof(GetGroupByGroupIdUseCase),
                request.GroupUid);

            return UseCaseResponse<GroupReadModelResponse>.Failure(Failure, "Invalid group identifier.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{UseCase} execution failed unexpectedly.",
                nameof(GetEstablishmentsUseCase));

            return UseCaseResponse<GroupReadModelResponse>.Failure(Failure, "An unexpected error occurred.");
        }
    }

    private static GroupReadModelResponse Failure => new()
    {
        Group = null
    };
}
