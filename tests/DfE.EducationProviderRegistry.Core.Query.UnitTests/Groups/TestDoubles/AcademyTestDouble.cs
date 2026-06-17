using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class AcademyTestDouble
{
    private const string AcademyNamePrefix = "Test Academy";

    public static IReadOnlyCollection<Academy> Create(int count = 1)
    {
        Faker faker = new();

        List<Academy> academies = new(count);

        for (int i = 0; i < count; i++)
        {
            academies.Add(CreateWith(
                urn: GenerateUrn(faker),
                name: $"{AcademyNamePrefix} {i}"));
        }

        return academies.AsReadOnly();
    }

    public static Academy CreateWith(
        string urn = "100000",
        string name = AcademyNamePrefix)
    {
        return new(
            new AcademyId(
                new UniqueReferenceNumber(urn)),
            new AcademyName(name)
        );
    }

    public static IReadOnlyCollection<Academy> CreateWith(
        params (string Urn, string Name)[] inputs)
    {
        List<Academy> academies = new(inputs.Length);

        foreach ((string urn, string name) in inputs)
        {
            academies.Add(CreateWith(urn, name));
        }

        return academies.AsReadOnly();
    }

    private static string GenerateUrn(Faker faker)
    {
        // 6-digit URN (safe default within 5–7 rule)
        int number = faker.Random.Number(100000, 999999);
        return number.ToString();
    }
}
