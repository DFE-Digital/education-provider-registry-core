using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class AcademyNameTestDoubles
{
    public static AcademyName Create(string value = "Academy")
    {
        return new(value);
    }
}
