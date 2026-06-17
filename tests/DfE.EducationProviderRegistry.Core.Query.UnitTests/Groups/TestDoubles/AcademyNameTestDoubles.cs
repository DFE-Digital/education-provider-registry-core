using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class AcademyNameTestDoubles
{
    public static AcademyName Create(string value = "Academy")
    {
        return new(value);
    }
}
