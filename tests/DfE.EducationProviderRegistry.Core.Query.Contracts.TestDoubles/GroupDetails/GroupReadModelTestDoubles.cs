using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;

public static class GroupReadModelTestDoubles
{
    public static GroupReadModel Stub()
    {
        return new GroupReadModel()
        {
            Name = "Test Group",
            GroupId = "group-1",
            GroupUID = 123,
            UKPRN = "10000001",
            CompaniesHouseId = "CH123456",
            Address = "1 Test Street",
            Status = "Open",
            Type = "Multi-academy trust",
            Academies = AcademyTestDoubles.Create(count: 7),
            Members = MemberReadModelTestDoubles.Create(count: 9),
            Trustees = TrusteeReadModelTestDoubles.Create(count: 13)
        };
    }
}
