using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.GroupDetails;

public static class MemberReadModelTestDoubles
{
    private static readonly Faker<MemberReadModel> _faker =
        new Faker<MemberReadModel>()
            .RuleFor(model => model.Identifier, faker => faker.Random.AlphaNumeric(8))
            .RuleFor(model => model.FullName, faker => faker.Name.FullName())
            .RuleFor(model => model.StartDate, faker => faker.Date.Past());

    public static MemberReadModel Create()
    {
        return _faker.Generate();
    }

    public static MemberReadModel Stub(
        string identifier = "M001",
        string fullName = "John Smith",
        DateTime? startDate = null)
    {
        return new MemberReadModel
        {
            Identifier = identifier,
            FullName = fullName,
            StartDate = startDate ?? new DateTime(2024, 1, 1)
        };
    }

    public static IReadOnlyCollection<MemberReadModel> Create(int count = 1)
    {
        List<MemberReadModel> members = new(count);

        for (int i = 0; i < count; i++)
        {
            members.Add(MemberReadModelTestDoubles.Create());
        }

        return members.AsReadOnly();
    }
}
