using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupReadModelTestDoubles
{
    internal static GroupReadModel Stub()
    {
        return new()
        {
            Name = "Test Group",
            GroupId = "Id",
            GroupUID = 123,
            UKPRN = "ukprn",
            CompaniesHouseId = "A123",
            Address = "123 Test Street",
            Type = "Multi Academy Trust",
            Status = "Opened on 12 June 2005",
            Academies = [],
            Members = [],
            Trustees = []
        };
    }
}
