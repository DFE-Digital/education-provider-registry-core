using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupDtoTestDoubles
{
    internal static GroupDto StubGroupDto()
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
