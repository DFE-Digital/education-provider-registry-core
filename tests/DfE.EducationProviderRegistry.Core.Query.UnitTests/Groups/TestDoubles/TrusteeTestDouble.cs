using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class TrusteeTestDouble
{

    public static IReadOnlyCollection<Trustee> Create(int count = 1)
    {
        Faker faker = new();

        List<Trustee> trustees = new(count);

        for (int i = 0; i < count; i++)
        {
            trustees.Add(CreateWith(
                id: faker.Random.Guid().ToString(),
                name: faker.Person.FullName,
                startDate: faker.Date.Past()));
        }

        return trustees;
    }

    public static Trustee CreateWith(
        string id = "trustee-1",
        string name = "Test Trustee",
        DateTime? startDate = null)
    {
        return new(
            new GroupMemberIdentifier(id),
            new GroupMemberName(name),
            startDate ?? DateTime.UtcNow
        );
    }

    public static IReadOnlyCollection<Trustee> CreateWith(
        params (string Id, string Name, DateTime StartDate)[] inputs)
    {
        List<Trustee> trustees = new(inputs.Length);

        foreach ((string id, string name, DateTime startDate) in inputs)
        {
            trustees.Add(
                CreateWith(
                    id, name, startDate));
        }

        return trustees.AsReadOnly();
    }
}
