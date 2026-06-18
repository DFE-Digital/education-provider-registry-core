using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

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
                id: faker.Random.Number(1000000, 7777777).ToString(),
                name: faker.Person.FullName,
                startDate: faker.Date.Past()));
        }

        return trustees;
    }

    public static Trustee CreateWith(
        string id = "1234567",
        string name = "Test Trustee",
        DateTime? startDate = null,
        TrusteeTitleType titleType = TrusteeTitleType.Other)
    {
        return new(
            new GovernanceIdentifier(id),
            new Name(name),
            startDate ?? DateTime.UtcNow,
            CreateTrusteeTitle(titleType)
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

    public static TrusteeTitle CreateTrusteeTitle(TrusteeTitleType type = TrusteeTitleType.CFO)
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
