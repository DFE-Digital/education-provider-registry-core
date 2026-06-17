using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

internal static class CompaniesHouseIdTestDoubles
{
    public static CompaniesHouseId Create() => Create("CH123");

    public static CompaniesHouseId Create(string value)
    {
        return new CompaniesHouseId(value);
    }
}
