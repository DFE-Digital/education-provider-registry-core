using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record class GroupCharacteristics
{
    public GroupCharacteristics(Name name, SiteAddressModel address, GroupType type, GroupStatus? status)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(type);
        Name = name;
        Address = address;
        Type = type;
        Status = status;
    }

    public Name Name { get; }
    public SiteAddressModel Address { get; }
    public GroupStatus? Status { get; }
    public GroupType Type { get; }
}
