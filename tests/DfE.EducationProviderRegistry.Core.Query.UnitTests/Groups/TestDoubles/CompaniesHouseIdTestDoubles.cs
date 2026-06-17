using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class CompaniesHouseIdTestDoubles
{
    public static CompaniesHouseId Create()
    {
        return new CompaniesHouseId("CH123");
    }

    public static CompaniesHouseId Create(string value)
    {
        return new CompaniesHouseId(value);
    }
}
