using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles;

public static class CompaniesHouseIdTestDoubles
{
    public static CompaniesHouseId Stub() => Create("CH123");

    public static CompaniesHouseId Create(string value)
    {
        return new CompaniesHouseId(value);
    }
}
