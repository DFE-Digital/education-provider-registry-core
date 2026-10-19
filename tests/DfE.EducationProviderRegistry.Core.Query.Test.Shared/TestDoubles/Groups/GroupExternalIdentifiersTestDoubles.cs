using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupExternalIdentifiersTestDoubles
{
    public static GroupExternalIdentifiers Create()
        => new(
            UkprnTestDoubles.Stub(),
            CompaniesHouseIdTestDoubles.Stub());
}
