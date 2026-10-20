using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;

public static class TrusteeReadModelTestDoubles
{
    private const string TrusteeNamePrefix = "Test Trustee";

    public static TrusteeReadModel CreateWith(
        string id = "trustee-1",
        string fullName = "Test Trustee",
        DateTime? startDate = null,
        TrusteeTitleType? title = TrusteeTitleType.Chair)
    {
        return new TrusteeReadModel
        {
            Id = id,
            FullName = fullName,
            StartDate = startDate ?? new DateTime(2024, 1, 1),
            Title = title,
        };
    }

    public static IReadOnlyCollection<TrusteeReadModel> Create(int count = 1)
    {
        List<TrusteeReadModel> trustees = new(count);

        for (int i = 0; i < count; i++)
        {
            trustees.Add(
                TrusteeReadModelTestDoubles.CreateWith(
                    id: $"trustee-{i}",
                    fullName: $"{TrusteeNamePrefix} {i}"));
        }

        return trustees.AsReadOnly();
    }

    public static IReadOnlyCollection<TrusteeReadModel> CreateWith(
        params (string Id, string FullName)[] inputs)
    {
        List<TrusteeReadModel> trustees = new(inputs.Length);

        foreach ((string id, string fullName) in inputs)
        {
            trustees.Add(
                CreateWith(
                    id,
                    fullName));
        }

        return trustees.AsReadOnly();
    }
}
