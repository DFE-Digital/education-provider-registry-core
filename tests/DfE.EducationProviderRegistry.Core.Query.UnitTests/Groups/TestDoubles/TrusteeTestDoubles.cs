using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class TrusteeTestDoubles
{
    public static IReadOnlyCollection<Trustee> Create(int count = 1)
    {
        Faker faker = new();

        List<Trustee> trustees = new(count);

        for (int i = 0; i < count; i++)
        {
            trustees.Add(CreateWith(
                id: faker.Random.Guid(),
                name: faker.Person.FullName,
                startDate: faker.Date.Past()));
        }

        return trustees;
    }

    public static Trustee CreateWith(
        Guid? id = null,
        string name = "Test Trustee",
        DateTime? startDate = null,
        TrusteeTitleType titleType = TrusteeTitleType.Other)
    {
        string identifier = id?.ToString() ?? Guid.NewGuid().ToString();

        return new(
            new GroupMemberIdentifier(identifier),
            new GroupMemberName(name),
            startDate ?? DateTime.UtcNow,
            CreateTrusteeTitle(titleType)
        );
    }

    public static IReadOnlyCollection<Trustee> CreateWith(
        params (Guid Id, string Name, DateTime StartDate)[] inputs)
    {
        List<Trustee> trustees = new(inputs.Length);

        foreach ((Guid id, string name, DateTime startDate) in inputs)
        {
            trustees.Add(
                CreateWith(
                    id, name, startDate));
        }

        return trustees.AsReadOnly();
    }

    private static TrusteeTitle CreateTrusteeTitle(TrusteeTitleType type)
    {
        return type switch
        {
            TrusteeTitleType.Chair => new("chair"),
            TrusteeTitleType.CFO => new("cfo"),
            TrusteeTitleType.AccountingOfficer => new("accounting"),
            _ => new("other")
        };
    }
}
