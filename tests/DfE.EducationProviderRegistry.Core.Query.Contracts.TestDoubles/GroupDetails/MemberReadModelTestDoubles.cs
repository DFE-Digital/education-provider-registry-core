using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;

public static class MemberReadModelTestDoubles
{
    private static readonly Faker<MemberReadModel> _faker =
        new Faker<MemberReadModel>()
            .RuleFor(model => model.Identifier, faker => faker.Random.AlphaNumeric(8))
            .RuleFor(model => model.FullName, faker => faker.Name.FullName())
            .RuleFor(model => model.Title, faker => faker.Name.Prefix())
            .RuleFor(model => model.AppointedBy, faker => faker.PickRandom(
                "Appointed by academy members",
                "Appointed by foundation/Trust",
                "Appointed by GB/board"))
            .RuleFor(
                model => model.StartDate,
                faker => DateOnly.FromDateTime(faker.Date.Past()))
            .RuleFor(
                model => model.EndDate,
                faker => DateOnly.FromDateTime(faker.Date.Future()));

    public static MemberReadModel Stub(
        string identifier = "M001",
        string fullName = "John Smith",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        string? title = "Mr",
        string? appointedBy = "Appointed by academy members")
    {
        return new MemberReadModel
        {
            Identifier = identifier,
            FullName = fullName,
            Title = title,
            AppointedBy = appointedBy,
            StartDate = startDate ?? new DateOnly(2024, 1, 1),
            EndDate = endDate
        };
    }

    public static MemberReadModel Create()
    {
        return _faker.Generate();
    }

    public static IReadOnlyCollection<MemberReadModel> Create(int count)
    {
        List<MemberReadModel> members = new(count);

        for (int i = 0; i < count; i++)
        {
            members.Add(Create());
        }

        return members.AsReadOnly();
    }
}
