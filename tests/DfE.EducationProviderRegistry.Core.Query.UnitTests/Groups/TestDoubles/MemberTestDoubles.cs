using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

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
                id: faker.Random.Number(1000000, 7777777).ToString(),
                name: faker.Person.FullName,
                startDate: faker.Date.Past()));
        }

        return members.AsReadOnly();
    }

    public static Member CreateWith(
        string id = "1234567",
        string name = "Test Member",
        DateTime? startDate = null)
    {
        return new(
            new GovernanceIdentifier(id),
            new Name(name),
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
