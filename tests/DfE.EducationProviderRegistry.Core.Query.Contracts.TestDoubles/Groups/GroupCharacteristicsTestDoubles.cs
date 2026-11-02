using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Shared;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class GroupCharacteristicsTestDoubles
{
    public static GroupCharacteristics Create()
    {
        return new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Stub(),
            GroupTypeTestDoubles.Stub(),
            GroupStatusTestDoubles.Create());
    }
}
