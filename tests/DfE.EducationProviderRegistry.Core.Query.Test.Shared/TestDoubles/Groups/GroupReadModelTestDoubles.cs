using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupReadModelTestDoubles
{
    public static GroupReadModel Stub()
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
