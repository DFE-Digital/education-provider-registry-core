using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

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
