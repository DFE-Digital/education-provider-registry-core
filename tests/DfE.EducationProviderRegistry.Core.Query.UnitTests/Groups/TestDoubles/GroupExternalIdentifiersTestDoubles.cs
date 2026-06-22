using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupExternalIdentifiersTestDoubles
{
    internal static GroupExternalIdentifiers Create()
        => new(
            UkprnTestDoubles.Create(),
            CompaniesHouseIdTestDoubles.Create());
}
