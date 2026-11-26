using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;

public static class TrusteeReadModelTestDoubles
{
    private const string TrusteeNamePrefix = "Test Trustee";

    public static TrusteeReadModel CreateWith(
        string id = "trustee-1",
        string fullName = "Test Trustee",
        string role = "Trustee",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        string? title = "Mr",
        string? appointedBy = "Appointed by academy members")
    {
        return new TrusteeReadModel
        {
            Id = id,
            FullName = fullName,
            Role = role,
            StartDate = startDate ?? new DateOnly(2024, 1, 1),
            EndDate = endDate,
            Title = title,
            AppointedBy = appointedBy
        };
    }

    public static IReadOnlyCollection<TrusteeReadModel> CreateWith(
        params (string Id, string FullName)[] inputs)
    {
        List<TrusteeReadModel> trustees = new(inputs.Length);

        foreach ((string id, string fullName) in inputs)
        {
            trustees.Add(
                CreateWith(
                    id: id,
                    fullName: fullName));
        }

        return trustees.AsReadOnly();
    }

    public static IReadOnlyCollection<TrusteeReadModel> Create(int count = 1)
    {
        List<TrusteeReadModel> trustees = new(count);

        for (int i = 0; i < count; i++)
        {
            trustees.Add(
                CreateWith(
                    id: $"trustee-{i}",
                    fullName: $"{TrusteeNamePrefix} {i}"));
        }

        return trustees.AsReadOnly();
    }
}
