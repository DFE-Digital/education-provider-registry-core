using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;

internal sealed class EstablishmentDtoBuilder
{
    private readonly EstablishmentDto _dto = new()
    {
        URN = "123456",
        UKPRN = "10000123",
        UPPN = "20000234",
        Name = "Test School",
        Number = "123",
        Status = "Open",
        Type = "Academy",
        PhaseOfEducation = "Primary",
        OpenDate = DateTime.UtcNow,
        ReasonEstablishmentOpened = "New school",
        Address = new AddressDto
        {
            Street = "Street",
            Town = "Town",
            County = "County",
            Postcode = "AB1 2CD"
        },
        Governors = new List<GovernorDto>()
    };

    public EstablishmentDtoBuilder WithUrn(string urn)
    {
        _dto.URN = urn;
        return this;
    }

    public EstablishmentDtoBuilder WithUkprn(string ukprn)
    {
        _dto.UKPRN = ukprn;
        return this;
    }

    public EstablishmentDtoBuilder WithUprn(string uprn)
    {
        _dto.UPPN = uprn;
        return this;
    }

    public EstablishmentDtoBuilder WithName(string name)
    {
        _dto.Name = name;
        return this;
    }

    public EstablishmentDtoBuilder WithNumber(string number)
    {
        _dto.Number = number;
        return this;
    }

    public EstablishmentDtoBuilder WithStatus(string status)
    {
        _dto.Status = status;
        return this;
    }

    public EstablishmentDtoBuilder WithType(string type)
    {
        _dto.Type = type;
        return this;
    }

    public EstablishmentDtoBuilder WithPhase(string phase)
    {
        _dto.PhaseOfEducation = phase;
        return this;
    }

    public EstablishmentDtoBuilder WithOpenDate(DateTime date)
    {
        _dto.OpenDate = date;
        return this;
    }

    public EstablishmentDtoBuilder WithOpenReason(string reason)
    {
        _dto.ReasonEstablishmentOpened = reason;
        return this;
    }

    public EstablishmentDtoBuilder WithCloseInfo(DateTime? date, string? reason)
    {
        _dto.CloseDate = date;
        _dto.ReasonEstablishmentClosed = reason;
        return this;
    }

    public EstablishmentDtoBuilder WithAddress(string street, string town, string county, string postcode)
    {
        _dto.Address = new AddressDto
        {
            Street = street,
            Town = town,
            County = county,
            Postcode = postcode
        };
        return this;
    }

    public EstablishmentDtoBuilder WithGovernors(IEnumerable<GovernorDto> governors)
    {
        _dto.Governors = governors.ToList();
        return this;
    }

    public EstablishmentDto Build() => _dto;
}


internal static class EstablishmentDtoFactory
{
    public static EstablishmentDto Create()
    {
        return CreateMany(1).Single();
    }

    public static IReadOnlyCollection<EstablishmentDto> CreateMany(int count)
    {
        List<EstablishmentDto> list = new List<EstablishmentDto>(count);

        for (int i = 0; i < count; i++)
        {
            list.Add(
                new EstablishmentDtoBuilder()
                    .WithUrn((100000 + i).ToString())
                    .Build());
        }

        return list.AsReadOnly();
    }
}
