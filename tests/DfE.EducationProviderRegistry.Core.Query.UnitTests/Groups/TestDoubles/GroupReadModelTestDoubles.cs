using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupReadModelTestDoubles
{
    internal static GroupReadModel Stub()
    {
        return new()
        {
            GroupId = "Id",
            GroupUID = 123,
            CompaniesHouseId = "A123",
            Academies = [],
            Members = [],
            Trustees = []
        };
    }
}
