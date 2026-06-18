using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;

internal sealed class FakeGroupsRepository : IGroupsRepository
{
    public Task<Group?> GetGroupByGroupIdAsync(GroupId groupId, CancellationToken cancellationToken = default)
    {
        return null!;
    }
}
