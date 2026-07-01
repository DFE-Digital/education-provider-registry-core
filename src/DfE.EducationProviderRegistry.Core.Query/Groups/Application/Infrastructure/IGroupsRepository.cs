using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

public interface IGroupsRepository
{
    Task<Group?> GetGroupByGroupUidAsync(GroupUID groupUid, CancellationToken cancellationToken = default);
}
