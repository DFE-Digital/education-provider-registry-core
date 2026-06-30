using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;

internal sealed class GroupsRepository : IGroupsRepository
{
    private readonly ILogger<GroupsRepository> _logger;
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly IMapper<GroupRecord, Group> _mapper;

    public GroupsRepository(
        ILogger<GroupsRepository> logger,
        EducationProviderRegistryDbContext dbContext,
        IMapper<GroupRecord, Group> mapper)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(mapper);

        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Group?> GetGroupByGroupUidAsync(GroupUID groupUid, CancellationToken cancellationToken = default)
    {
        GroupRecord? group =
            await _dbContext.GroupRecord
                .SingleOrDefaultAsync((group) =>
                    group.GroupId == groupUid.Value, cancellationToken);

        if (group is null)
        {
            _logger.LogWarning("Could not find Group with GroupId {GroupId}", groupUid.Value);
            return null;
        }

        return _mapper.Map(group);
    }
}
