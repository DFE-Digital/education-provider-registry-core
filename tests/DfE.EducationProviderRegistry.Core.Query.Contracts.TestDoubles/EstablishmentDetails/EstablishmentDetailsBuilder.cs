using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;

public sealed class EstablishmentDetailsBuilder
{
    private static int _urnCounter = 100_500;
    private int? _urn;
    private string? _ukprn;
    private string? _dfeNumber;

    private string? _website;
    private string? _telephoneNumber;

    private EstablishmentStatus? _status;
    private DateOnly? _statusEffectiveDate;
    private string? _siteName;
    private string? _addressLine1;
    private string? _addressLine2;
    private string? _town;
    private string? _county;
    private string? _postcode;

    private string? _groupName;
    private string? _groupCode;
    private string? _headteacher;
    private DateOnly? _ofstedDate;
    private string? _ofstedUrl;
    private string? _senProvision;
    private string? _localAuthorityName;
    private LocalAuthorityCode _localAuthorityCode;
    private string? _ageRange;
    private string? _gender;
    private int? _pupilCount;
    private string? _religious;
    private string? _phase;
    private string? _type;

    public static EstablishmentDetailsBuilder Create() => new();

    public EstablishmentDetailsBuilder()
    {
        _status = EstablishmentStatus.Open;
        _statusEffectiveDate = new(2026, 03, 01);
        _localAuthorityCode = new(100);
    }

    public EstablishmentDetailsBuilder WithUrn(int? urn)
    {
        _urn = urn;
        return this;
    }

    public EstablishmentDetailsBuilder WithUkprn(string? ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    public EstablishmentDetailsBuilder WithDfeNumber(string? dfeNumber)
    {
        _dfeNumber = dfeNumber;
        return this;
    }

    public EstablishmentDetailsBuilder WithWebsite(string? website)
    {
        _website = website;
        return this;
    }

    public EstablishmentDetailsBuilder WithTelephoneNumber(string telephoneNumber)
    {
        _telephoneNumber = telephoneNumber;
        return this;
    }

    public EstablishmentDetailsBuilder WithStatus(EstablishmentStatus? status)
    {
        _status = status;
        return this;
    }

    public EstablishmentDetailsBuilder WithStatusDate(DateOnly? date)
    {
        _statusEffectiveDate = date;
        return this;
    }

    public EstablishmentDetailsBuilder WithSiteName(string? name)
    {
        _siteName = name;
        return this;
    }

    public EstablishmentDetailsBuilder WithAddressLine1(string? line1)
    {
        _addressLine1 = line1;
        return this;
    }

    public EstablishmentDetailsBuilder WithAddressLine2(string? line2)
    {
        _addressLine2 = line2;
        return this;
    }

    public EstablishmentDetailsBuilder WithTown(string? town)
    {
        _town = town;
        return this;
    }

    public EstablishmentDetailsBuilder WithCounty(string? county)
    {
        _county = county;
        return this;
    }

    public EstablishmentDetailsBuilder WithPostcode(string? postCode)
    {
        _postcode = postCode;
        return this;
    }

    public EstablishmentDetailsBuilder WithGroupName(string? name)
    {
        _groupName = name;
        return this;
    }

    public EstablishmentDetailsBuilder WithGroupCode(string? code)
    {
        _groupCode = code;
        return this;
    }

    public EstablishmentDetailsBuilder WithHeadteacher(string? headteacher)
    {
        _headteacher = headteacher;
        return this;
    }

    public EstablishmentDetailsBuilder WithOfstedDate(DateOnly? date)
    {
        _ofstedDate = date;
        return this;
    }

    public EstablishmentDetailsBuilder WithOfstedReportUrl(string? url)
    {
        _ofstedUrl = url;
        return this;
    }

    public EstablishmentDetailsBuilder WithSenProvision(string? sen)
    {
        _senProvision = sen;
        return this;
    }

    public EstablishmentDetailsBuilder WithLocalAuthorityName(string laName)
    {
        _localAuthorityName = laName;
        return this;
    }

    public EstablishmentDetailsBuilder WithAgeRange(int low, int high)
    {
        _ageRange = $"{low} to {high}";
        return this;
    }

    public EstablishmentDetailsBuilder WithGender(string gender)
    {
        _gender = gender;
        return this;
    }

    public EstablishmentDetailsBuilder WithPupilCount(int count)
    {
        _pupilCount = count;
        return this;
    }

    public EstablishmentDetailsBuilder WithReligiousCharacter(string religious)
    {
        _religious = religious;
        return this;
    }

    public EstablishmentDetailsBuilder WithType(string type)
    {
        _type = type;
        return this;
    }

    public EstablishmentDetailsBuilder WithPhase(string phase)
    {
        _phase = phase;
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
            Identifiers = new
                EstablishmentIdentifiersModel(
                    urn: new EstablishmentUrnModel(
                        new UniqueReferenceNumber(urn)),
                    ukprn: _ukprn is null ? null : new Ukprn(_ukprn),
                    dfeNumber: _dfeNumber is null ? null : new DfeNumber(_dfeNumber)),
            ContactDetails = new EstablishmentContactDetails(
                Website: _website ?? string.Empty,
                TelephoneNumber: _telephoneNumber ?? string.Empty),
            Headteacher = _headteacher,
            Ofsted = new(_ofstedDate, _ofstedUrl),
            Address = new(_siteName, _addressLine1, _addressLine2, _town, _county, _postcode),
            Status = _status is null ? null : new(_status.Value, _statusEffectiveDate),
            Group = _groupName is null || _groupCode is null ?
                null :
                new(name: _groupName, uid: _groupCode),
            SenProvision = _senProvision,
            LocalAuthority = _localAuthorityName is null ? null : new(_localAuthorityName, _localAuthorityCode),
            AgeRange = _ageRange,
            Gender = _gender,
            Pupils = _pupilCount is null ? null : new(_pupilCount.Value),
            ReligiousCharacter = _religious,
            Phase = new(_phase),
            Type = _type is null ? null : new(_type)
        };
    }
}
