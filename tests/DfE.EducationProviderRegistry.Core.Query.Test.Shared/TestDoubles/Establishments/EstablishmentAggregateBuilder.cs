using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Establishments;

public sealed class EstablishmentAggregateBuilder
{
    private readonly EstablishmentAggregate _aggregate;

    public static EstablishmentAggregateBuilder Create() => new();

    public EstablishmentAggregateBuilder()
    {
        _aggregate = new EstablishmentAggregate
        {
            Urn = "123456",
            Name = "Test School",
            // EstablishmentNumber = "123",

            //StatusCode = (int)EstablishmentStatus.Open,

            //EstablishmentTypeName = "Academy",

            //EducationPhaseName = "Primary",

            //OpenedDate = new DateOnly(2000, 1, 1),

            //ClosedDate = new DateOnly(2020, 1, 1),

            //GroupUid = 999,
            //GroupCode = "GRP001",
            //GroupName = "Test Group",
            //GroupTypeName = "Multi-academy trust",
            //GroupOpenDate = new DateOnly(2010, 1, 1),

            //SiteName = "Main Site",
            //AddressLine1 = "1 Test Street",
            //AddressLine2 = "Test Area",
            //Town = "Test Town",
            //County = "Test County",
            //Postcode = "TE1 1ST",

            //LocalAuthorityCode = 123,
            //LocalAuthorityName = "Test Authority",

            //StatutoryLowAge = 5,
            //StatutoryHighAge = 11,

            //Gender = "Mixed",

            //ReligiousCharacter = "Church of England",

            //OfstedInspectionDate = new DateOnly(2024, 1, 1),
            //OfstedReportUrl = "https://reports.ofsted.gov.uk/test",

            //HeadteacherName = "John Smith",

            //SenProvision = "SEN Unit",

            //Website = "https://www.testschool.com",
            //TelephoneNumber = "0123456789"
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

    public EstablishmentAggregateBuilder WithSiteName(string? name)
    {
        _aggregate.SiteName = name;
        return this;
    }

    public EstablishmentAggregateBuilder WithAddressLine1(string? line1)
    {
        _aggregate.AddressLine1 = line1;
        return this;
    }

    public EstablishmentAggregateBuilder WithAddressLine2(string? line2)
    {
        _aggregate.AddressLine2 = line2;
        return this;
    }

    public EstablishmentAggregateBuilder WithTown(string? town)
    {
        _aggregate.Town = town;
        return this;
    }

    public EstablishmentAggregateBuilder WithCounty(string? county)
    {
        _aggregate.County = county;
        return this;
    }

    public EstablishmentAggregateBuilder WithPostcode(string? postCode)
    {
        _aggregate.Postcode = postCode;
        return this;
    }

    public EstablishmentAggregateBuilder WithGroupName(string? name)
    {
        _aggregate.GroupName = name;
        return this;
    }

    public EstablishmentAggregateBuilder WithGroupCode(string? code)
    {
        _aggregate.GroupCode = code;
        return this;
    }

    public EstablishmentAggregateBuilder WithGroupUid(long? uid)
    {
        _aggregate.GroupUid = uid;
        return this;
    }

    public EstablishmentAggregateBuilder WithWebsite(string? website)
    {
        _aggregate.Website = website;
        return this;
    }

    public EstablishmentAggregateBuilder WithTelephoneNumber(string? telephoneNumber)
    {
        _aggregate.TelephoneNumber = telephoneNumber;
        return this;
    }

    public EstablishmentAggregateBuilder WithLocalAuthorityCode(int? localAuthorityCode)
    {
        _aggregate.LocalAuthorityCode = localAuthorityCode;
        return this;
    }

    public EstablishmentAggregateBuilder WithLocalAuthorityName(string localAuthorityName)
    {
        _aggregate.LocalAuthorityName = localAuthorityName;
        return this;
    }

    public EstablishmentAggregateBuilder WithLowAge(int? lowAge)
    {
        _aggregate.StatutoryLowAge = lowAge;
        return this;
    }

    public EstablishmentAggregateBuilder WithHighAge(int? highAge)
    {
        _aggregate.StatutoryHighAge = highAge;
        return this;
    }

    public EstablishmentAggregateBuilder WithEstablishmentNumber(string? establishmentNumber)
    {
        _aggregate.EstablishmentNumber = establishmentNumber;
        return this;
    }

    public EstablishmentAggregateBuilder WithStatusCode(EstablishmentStatus? code)
    {
        _aggregate.StatusCode = code is null ? null : (int)code;
        return this;
    }

    public EstablishmentAggregateBuilder WithOfstedInspectionDate(DateOnly? inspectionDate)
    {
        _aggregate.OfstedInspectionDate = inspectionDate;
        return this;
    }

    public EstablishmentAggregateBuilder WithOfstedReportUrl(string? url)
    {
        _aggregate.OfstedReportUrl = url;
        return this;
    }

    public EstablishmentAggregate Build() => _aggregate;
}
