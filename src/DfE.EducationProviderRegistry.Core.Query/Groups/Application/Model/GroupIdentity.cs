namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupIdentity
{
    public GroupIdentity(GroupUID uid, GroupId? id)
    {
        Id = id;
        Uid = uid;
    }

    public GroupUID Uid { get; }
    public GroupId? Id { get; }
}
