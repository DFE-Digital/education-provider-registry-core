using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class AcademyNameTestDoubles
{
    public static AcademyName Create(string value = "Academy")
    {
        return new(value);
    }
}
