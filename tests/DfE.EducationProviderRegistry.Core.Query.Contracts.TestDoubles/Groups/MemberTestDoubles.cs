using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class MemberTestDoubles
{
    public static IReadOnlyCollection<Member> Create(int count = 1)
    {
        Faker faker = new();

        List<Member> members = new(count);

        for (int i = 0; i < count; i++)
        {
            members.Add(
                CreateWith(
                    id: faker.Random.Number(1000000, 7777777).ToString(),
                    name: faker.Person.FullName,
                    startDate: DateOnly.FromDateTime(faker.Date.Past()),
                    title: faker.Name.Prefix(),
                    appointedBy: faker.PickRandom(
                        "Appointed by academy members",
                        "Appointed by foundation/Trust",
                        "Appointed by GB/board")));
        }

        return members.AsReadOnly();
    }

    public static Member CreateWith(
        string id = "1234567",
        string name = "Test Member",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        string? title = "Mr",
        string? appointedBy = "Appointed by academy members")
    {
        return new(
            new GovernanceIdentifier(id),
            new Name(name),
            startDate ?? new DateOnly(2024, 1, 1),
            endDate,
            title,
            appointedBy);
    }

    public static IReadOnlyCollection<Member> CreateWith(
        params (string Id, string Name, DateOnly StartDate)[] inputs)
    {
        List<Member> members = new(inputs.Length);

        foreach ((string id, string name, DateOnly startDate) in inputs)
        {
            members.Add(
                CreateWith(
                    id: id,
                    name: name,
                    startDate: startDate));
        }

        return members.AsReadOnly();
    }
}
