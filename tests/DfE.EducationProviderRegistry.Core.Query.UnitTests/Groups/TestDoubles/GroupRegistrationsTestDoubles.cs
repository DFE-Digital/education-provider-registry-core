using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupRegistrationsTestDoubles
{
    internal static GroupRegistrations Create()
        => new(
            UkprnTestDoubles.Create(),
            CompaniesHouseIdTestDoubles.Create());
}
