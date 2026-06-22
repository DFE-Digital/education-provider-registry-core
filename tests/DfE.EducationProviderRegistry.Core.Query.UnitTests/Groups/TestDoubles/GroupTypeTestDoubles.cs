using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupTypeTestDoubles
{
    internal static GroupType Create() => Create("MAT");
    internal static GroupType Create(string input) => new(input);
}
