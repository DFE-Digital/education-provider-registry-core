using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;

public sealed class EstablishmentDetailsBuilder
{
    private static int _urnCounter = 100_500;
    private string _website = string.Empty;
    private int? _urn;
    private EstablishmentStatus _status = EstablishmentStatus.Open;
    private DateOnly? _statusEffectiveDate = new(2026, 03, 01);

    public EstablishmentDetailsBuilder WithUrn(int urn)
    {
        _urn = urn;
        return this;
    }

    public EstablishmentDetailsBuilder WithWebsite(string website)
    {
        _website = website ?? string.Empty;
        return this;
    }

    public EstablishmentDetailsBuilder WithStatus(EstablishmentStatus status)
    {
        _status = status;
        return this;
    }

    public EstablishmentDetailsBuilder WithDate(DateOnly? date)
    {
        _statusEffectiveDate = date;
        return this;
    }

    public EstablishmentDetailsModel Build()
    {
        string urn =
            (_urn is not null ?
                _urn.Value : Interlocked.Increment(ref _urnCounter)).ToString();

        return new()
        {
            Name = new EstablishmentNameModel("Test Establishment"),
            Identifiers = new EstablishmentIdentifiersModel(
                urn: new EstablishmentUrnModel(
                    new UniqueReferenceNumber(urn)),
                ukprn: new Ukprn("12345678"),
                dfeNumber: null),
            ContactDetails = new EstablishmentContactDetails(
                Website: _website,
                TelephoneNumber: string.Empty),
            Status = new(_status, _statusEffectiveDate)
        };
    }

    public static EstablishmentDetailsBuilder Create() => new();
}
