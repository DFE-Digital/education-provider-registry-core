using System.Security.Cryptography;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public sealed class GroupAggregateBuilder
{
    private long _aggregateId;
    private string _groupId;
    private long _groupUid;
    private string _name;
    private string? _ukprn;
    private string? _companiesHouseNumber;
    private string? _groupTypeName;
    private string? _siteName;
    private string? _addressLine1;
    private string? _addressLine2;
    private string? _town;
    private string? _county;
    private string? _postcode;
    private string? _statusLabel;
    private DateOnly? _statusEffectiveDate;

    public GroupAggregateBuilder()
    {
        _aggregateId = RandomNumberGenerator.GetInt32(999_999);
        _groupId = GroupIdTestDoubles.Stub().Value;
        _groupUid = GroupUIDTestDoubles.Stub().Value;
        _ukprn = UkprnTestDoubles.Stub().Value;
        _name = "Test Group";
    }

    public static GroupAggregateBuilder Create() => new();

    public GroupAggregateBuilder WithAggregateId(long id)
    {
        _aggregateId = id;
        return this;
    }

    public GroupAggregateBuilder WithGroupId(string groupId)
    {
        _groupId = groupId;
        return this;
    }

    public GroupAggregateBuilder WithGroupUid(long groupUid)
    {
        _groupUid = groupUid;
        return this;
    }

    public GroupAggregateBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public GroupAggregateBuilder WithUkprn(string? ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    public GroupAggregateBuilder WithCompaniesHouseNumber(string? companiesHouseNumber)
    {
        _companiesHouseNumber = companiesHouseNumber;
        return this;
    }

    public GroupAggregateBuilder WithGroupType(string? groupType)
    {
        _groupTypeName = groupType;
        return this;
    }

    public GroupAggregateBuilder WithSiteName(string? name)
    {
        _siteName = name;
        return this;
    }

    public GroupAggregateBuilder WithAddressLine1(string? line1)
    {
        _addressLine1 = line1;
        return this;
    }

    public GroupAggregateBuilder WithAddressLine2(string? line2)
    {
        _addressLine2 = line2;
        return this;
    }

    public GroupAggregateBuilder WithTown(string? town)
    {
        _town = town;
        return this;
    }

    public GroupAggregateBuilder WithCounty(string? county)
    {
        _county = county;
        return this;
    }

    public GroupAggregateBuilder WithPostcode(string? postCode)
    {
        _postcode = postCode;
        return this;
    }

    public GroupAggregateBuilder WithStatus(
        string? label,
        DateOnly? effectiveDate)
    {
        _statusLabel = label;
        _statusEffectiveDate = effectiveDate;

        return this;
    }

    public GroupAggregate Build()
    {
        return new()
        {
            GroupAggregateId = _aggregateId,
            GroupId = _groupId,
            Name = _name,
            GroupUid = _groupUid,
            Ukprn = _ukprn,
            CompaniesHouseNumber = _companiesHouseNumber,
            GroupTypeName = _groupTypeName,
            SiteName = _siteName,
            AddressLine1 = _addressLine1,
            AddressLine2 = _addressLine2,
            Town = _town,
            County = _county,
            Postcode = _postcode,
            GroupStatusLabel = _statusLabel,
            GroupStatusEffectiveDate = _statusEffectiveDate
        };
    }
}
