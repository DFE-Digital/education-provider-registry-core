using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupTypeTestDoubles
{
    public static GroupType Stub() => Create("MAT");
    public static GroupType Create(string input) => new(input);
}
