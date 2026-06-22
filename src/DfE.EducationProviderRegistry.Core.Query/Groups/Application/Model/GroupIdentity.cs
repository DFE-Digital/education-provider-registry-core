namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupIdentity
{
    public GroupIdentity(GroupId id, GroupUID uid)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
        Uid = uid;
    }

    public GroupId Id { get; }
    public GroupUID Uid { get; }
}
