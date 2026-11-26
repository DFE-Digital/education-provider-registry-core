using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;

internal sealed class GroupsRepository : IGroupsRepository
{
    private readonly ILogger<GroupsRepository> _logger;
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly IMapper<GroupMappingContext, Group> _mapper;

    public GroupsRepository(
        ILogger<GroupsRepository> logger,
        EducationProviderRegistryDbContext dbContext,
        IMapper<GroupMappingContext, Group> mapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(mapper);

        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Group?> GetGroupByGroupUidAsync(
        GroupUID groupUid,
        CancellationToken cancellationToken = default)
    {
        GroupAggregate? group =
            await _dbContext.GroupAggregate
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    g => g.GroupUid == groupUid.Value,
                    cancellationToken);

        if (group is null)
        {
            _logger.LogWarning(
                "Could not find Group with GroupUID {GroupUID}",
                groupUid.Value);

            return null;
        }

        IReadOnlyCollection<GroupEstablishmentAggregate> groupEstablishmentAggregates =
            await _dbContext.GroupEstablishmentAggregate
                .AsNoTracking()
                .Where(t => t.GroupUid == groupUid.Value)
                .ToArrayAsync(cancellationToken);

        IReadOnlyCollection<GroupGovernorAggregate> groupGovernorAggregates =
                await _dbContext.GroupGovernorAggregate
                    .AsNoTracking()
                    .Where(t => t.GroupUid == groupUid.Value)
                    .ToArrayAsync(cancellationToken);

        GroupMappingContext context =
            new(
                group,
                groupEstablishmentAggregates,
                groupGovernorAggregates);

        return _mapper.Map(context);
    }
}
