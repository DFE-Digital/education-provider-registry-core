using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupCharacteristics
{
    public GroupCharacteristics(Name name, Address address, GroupType type, GroupStatus status)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(status);
        Name = name;
        Address = address;
        Type = type;
        Status = status;
    }

    public Name Name { get; }
    public Address Address { get; }
    public GroupStatus Status { get; }
    public GroupType Type { get; }
}
