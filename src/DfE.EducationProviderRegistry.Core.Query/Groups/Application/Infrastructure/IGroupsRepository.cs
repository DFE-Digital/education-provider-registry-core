using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

public interface IGroupsRepository
{
    Task<Group?> GetGroupByGroupIdAsync(GroupId groupId, CancellationToken cancellationToken = default);
}
