using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class MemberTestDoubles
{
    public static IReadOnlyCollection<Member> Create(int count = 1)
    {
        Faker faker = new();

        List<Member> members = new(count);

        for (int i = 0; i < count; i++)
        {
            members.Add(CreateWith(
                id: faker.Random.Guid().ToString(),
                name: faker.Person.FullName,
                startDate: faker.Date.Past()));
        }

        return members.AsReadOnly();
    }

    public static Member CreateWith(
        string id = "member-1",
        string name = "Test Member",
        DateTime? startDate = null)
    {
        return new(
            new GroupMemberIdentifier(id),
            new GroupMemberName(name),
            startDate ?? DateTime.UtcNow
        );
    }

    public static IReadOnlyCollection<Member> CreateWith(
        params (string Id, string Name, DateTime StartDate)[] inputs)
    {
        List<Member> members = new(inputs.Length);

        foreach ((string id, string name, DateTime startDate) in inputs)
        {
            members.Add(CreateWith(id, name, startDate));
        }

        return members.AsReadOnly();
    }
}
