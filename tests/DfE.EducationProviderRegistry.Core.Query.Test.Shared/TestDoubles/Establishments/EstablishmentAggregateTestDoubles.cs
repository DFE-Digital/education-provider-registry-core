using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;

public static class EstablishmentAggregateTestDoubles
{
    public static EstablishmentAggregate Stub()
    {
        return StubMany(1).Single();
    }

    public static IReadOnlyCollection<EstablishmentAggregate> StubMany(int count)
    {
        Faker<EstablishmentAggregate> faker = new() { Locale = "en-GB" };
        List<EstablishmentAggregate> list = new(count);

        for (int i = 0; i < count; i++)
        {
            string urn = (100000 + i).ToString();

            list.Add(
                faker
                    .RuleFor(estab => estab.EstablishmentAggregateId, faker => faker.IndexFaker + 1)
                    .RuleFor(estab => estab.Urn, urn)
                    .RuleFor(estab => estab.Ukprn, faker => faker.Random.Bool() ? UkprnTestDoubles.Create($"value-{faker.IndexFaker}").Value : null)
                    .RuleFor(estab => estab.EstablishmentNumber, faker => EstablishmentNumberTestDoubles.Create().Value.ToString())
                    .RuleFor(estab => estab.Name, faker => faker.Company.CompanyName())
                    .RuleFor(estab => estab.StatusCode, faker => (int)faker.PickRandom<EstablishmentStatus>())
                    .RuleFor(estab => estab.StatusDate, faker => DateOnly.FromDateTime(faker.Date.Past()))
                    .RuleFor(estab => estab.EstablishmentTypeName, faker => faker.PickRandom("Academy", "Community School", "Free School"))
                    .RuleFor(estab => estab.EducationPhaseName, faker => faker.PickRandom("Primary", "Secondary", "All-through"))
                    .RuleFor(estab => estab.OpenedDate, faker => DateOnly.FromDateTime(faker.Date.Past(20)))
                    .RuleFor(estab => estab.ClosedDate, (faker, estab) => estab.StatusCode == (int)EstablishmentStatus.Closed
                        ? DateOnly.FromDateTime(faker.Date.Recent())
                        : null)
                    .RuleFor(estab => estab.GroupUid, faker => faker.Random.Bool() ? GroupUIDTestDoubles.Create().Value : null)
                    .RuleFor(estab => estab.GroupCode, faker => faker.Random.Bool() ? faker.Random.AlphaNumeric(6) : null)
                    .RuleFor(estab => estab.GroupName, faker => faker.Random.Bool() ? faker.Company.CompanyName() : null)
                    .RuleFor(estab => estab.GroupTypeName, faker => faker.Random.Bool() ? faker.PickRandom("MAT", "Federation") : null)
                    .RuleFor(estab => estab.GroupOpenDate, faker => faker.Random.Bool() ? DateOnly.FromDateTime(faker.Date.Past()) : null)
                    .RuleFor(estab => estab.SiteName, faker => faker.Address.SecondaryAddress())
                    .RuleFor(estab => estab.AddressLine1, faker => faker.Address.StreetAddress())
                    .RuleFor(estab => estab.AddressLine2, faker => faker.Random.Bool() ? faker.Address.BuildingNumber() : null)
                    .RuleFor(estab => estab.Town, faker => faker.Address.City())
                    .RuleFor(estab => estab.County, faker => faker.Address.County())
                    .RuleFor(estab => estab.Postcode, faker => faker.Address.ZipCode())
                    .RuleFor(estab => estab.LocalAuthorityCode, faker => faker.Random.Number(1, 999))
                    .RuleFor(estab => estab.LocalAuthorityName, faker => faker.Address.County())
                    .RuleFor(estab => estab.StatutoryLowAge, faker => faker.Random.Number(3, 11))
                    .RuleFor(estab => estab.StatutoryHighAge, faker => faker.Random.Number(11, 18))
                    .RuleFor(estab => estab.Gender, faker => faker.PickRandom("Mixed", "Boys", "Girls"))
                    .RuleFor(estab => estab.ReligiousCharacter, faker => faker.Random.Bool() ? "None" : null)
                    .RuleFor(estab => estab.OfstedInspectionDate, faker => DateOnly.FromDateTime(faker.Date.Past(5)))
                    .RuleFor(estab => estab.OfstedReportUrl, faker => faker.Internet.Url())
                    .RuleFor(estab => estab.HeadteacherName, faker => faker.Name.FullName())
                    .RuleFor(estab => estab.SenProvision, faker => faker.Random.Bool() ? "SEN Support" : null)
                    .RuleFor(estab => estab.Website, faker => faker.Internet.Url())
                    .RuleFor(estab => estab.TelephoneNumber, faker => faker.Phone.PhoneNumber())
            );
        }

        return list.AsReadOnly();
    }
}
