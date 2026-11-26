using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class TrusteeTestDoubles
{
    public static IReadOnlyCollection<Trustee> Create(int count = 1)
    {
        Faker faker = new();

        List<Trustee> trustees = new(count);

        for (int i = 0; i < count; i++)
        {
            trustees.Add(
                CreateWith(
                    id: faker.Random.Number(1000000, 7777777).ToString(),
                    name: faker.Person.FullName,
                    role: faker.PickRandom(
                        "Chair of Trustees",
                        "Trustee",
                        "Accounting Officer",
                        "Chief Financial Officer"),
                    startDate: DateOnly.FromDateTime(faker.Date.Past()),
                    endDate: DateOnly.FromDateTime(faker.Date.Future()),
                    title: faker.Name.Prefix(),
                    appointedBy: faker.PickRandom(
                        "Appointed by academy members",
                        "Appointed by foundation/Trust",
                        "Appointed by GB/board")));
        }

        return trustees;
    }

    public static Trustee CreateWith(
        string id = "1234567",
        string name = "Test Trustee",
        string role = "Trustee",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        string? title = "Mr",
        string? appointedBy = "Appointed by academy members")
    {
        return new(
            new GovernanceIdentifier(id),
            new Name(name),
            role,
            startDate ?? new DateOnly(2024, 1, 1),
            endDate,
            title,
            appointedBy);
    }

    public static IReadOnlyCollection<Trustee> CreateWith(
        params (string Id, string Name, string Role, DateOnly StartDate)[] inputs)
    {
        List<Trustee> trustees = new(inputs.Length);

        foreach ((string id, string name, string role, DateOnly startDate) in inputs)
        {
            trustees.Add(
                CreateWith(
                    id: id,
                    name: name,
                    role: role,
                    startDate: startDate));
        }

        return trustees.AsReadOnly();
    }
}
