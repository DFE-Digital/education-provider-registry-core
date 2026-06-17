using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class AcademyIdTestDoubles
{
    public static AcademyId Create(string value = "12345")
    {
        return new(
            new UniqueReferenceNumber(value));
    }
}
