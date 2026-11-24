using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class GroupTypeTestDoubles
{
    public static GroupType Stub() => Create(type: "MAT", code: "02");
    public static GroupType Create(string type, string code) => new(type, new GroupTypeCode(code));
}
