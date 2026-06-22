using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupCharacteristics
{
    public GroupCharacteristics(Address address, GroupType type, GroupStatus status)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(status);

        Address = address;
        Type = type;
        Status = status;
    }

    public Address Address { get; }
    public GroupStatus Status { get; }
    public GroupType Type { get; }
}
