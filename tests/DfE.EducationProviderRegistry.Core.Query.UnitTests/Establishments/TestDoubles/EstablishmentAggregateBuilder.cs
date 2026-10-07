using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles;

internal sealed class EstablishmentAggregateBuilder
{
    private readonly EstablishmentAggregate _aggregate;

    public EstablishmentAggregateBuilder()
    {
        _aggregate = new EstablishmentAggregate
        {
            Urn = "123456",
            Name = "Test School",
            EstablishmentNumber = "123",

            StatusCode = (int)EstablishmentStatus.Open,

            EstablishmentTypeName = "Academy",

            EducationPhaseName = "Primary",

            OpenedDate = new DateOnly(2000, 1, 1),

            ClosedDate = new DateOnly(2020, 1, 1),

            GroupUid = 999,
            GroupCode = "GRP001",
            GroupName = "Test Group",
            GroupTypeName = "Multi-academy trust",
            GroupOpenDate = new DateOnly(2010, 1, 1),

            SiteName = "Main Site",
            AddressLine1 = "1 Test Street",
            AddressLine2 = "Test Area",
            Town = "Test Town",
            County = "Test County",
            Postcode = "TE1 1ST",

            LocalAuthorityCode = 123,
            LocalAuthorityName = "Test Authority",

            StatutoryLowAge = 5,
            StatutoryHighAge = 11,

            Gender = "Mixed",

            ReligiousCharacter = "Church of England",

            OfstedInspectionDate = new DateOnly(2024, 1, 1),
            OfstedReportUrl = "https://reports.ofsted.gov.uk/test",

            HeadteacherIdentifier = "P123",
            HeadteacherName = "John Smith",

            SenProvision = "SEN Unit",

            Website = "https://www.testschool.com",
            TelephoneNumber = "0123456789"
        };
    }

    public EstablishmentAggregateBuilder WithUrn(string urn)
    {
        _aggregate.Urn = urn;
        return this;
    }

    public EstablishmentAggregateBuilder WithName(string name)
    {
        _aggregate.Name = name;
        return this;
    }

    public EstablishmentAggregateBuilder WithNumber(string number)
    {
        _aggregate.EstablishmentNumber = number;
        return this;
    }

    public EstablishmentAggregate Build() => _aggregate;
}
