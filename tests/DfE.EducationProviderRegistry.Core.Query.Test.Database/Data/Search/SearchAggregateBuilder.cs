using System.Reflection;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;

public sealed class SearchAggregateBuilder
{
    private static int _searchAggregateIdCounter = 100_000;
    private static int _provierIdCounter = 1000;

    private readonly SearchAggregate _searchAggregate;

    public SearchAggregateBuilder()
    {
        _searchAggregate = new SearchAggregate
        {
            SearchAggregateId = long.Parse(Interlocked.Increment(ref _searchAggregateIdCounter).ToString()),
            ProviderId = GenerateUniqueProviderId(),
            ProviderName = "Test Establishment",
            ProviderAddress = "1 Test Street, Test Town, Test County, TE1 1ST",
            LocalAuthorityName = "Test Authority",
            ProviderCategory = "Establishment",
            ProviderTypeId = 1, // Community school
            ProviderTypeName = "Multi-Academy Trust"
        };
    }

    // provider id must be a unique 4-7 character numeric else it'll fail to insert
    public SearchAggregateBuilder WithProviderId(int id)
    {
        _searchAggregate.ProviderId = id.ToString();
        return this;
    }

    public SearchAggregateBuilder WithLocalAuthorityName(string localAuthorityName)
    {
        _searchAggregate.LocalAuthorityName = localAuthorityName;

        return this;
    }

    public SearchAggregateBuilder WithProviderName(string name)
    {
        _searchAggregate.ProviderName = name;
        return this;
    }

    public SearchAggregateBuilder WithPostcode(string value)
    {
        _searchAggregate.Postcode = value;
        return this;
    }

    public SearchAggregateBuilder WithCounty(string county)
    {
        _searchAggregate.County = county;
        return this;
    }

    public SearchAggregateBuilder WithProviderTypeId(long typeId)
    {
        _searchAggregate.ProviderTypeId = typeId;
        return this;
    }

    public SearchAggregateBuilder WithProviderTypeName(string name)
    {
        _searchAggregate.ProviderTypeName = name;
        return this;
    }

    public SearchAggregateBuilder SetValue(string property, string value)
    {
        if (string.IsNullOrWhiteSpace(property))
        {
            throw new ArgumentException(
                "Property cannot be null or whitespace.",
                nameof(property));
        }

        PropertyInfo? propertyInfo =
            _searchAggregate.GetType().GetProperty(property) ??
                throw new ArgumentException($"Property '{property}' does not exist on {nameof(Establishment)}.", nameof(property));

        if (!propertyInfo.CanWrite)
        {
            throw new ArgumentException(
                $"Property '{property}' is read-only.",
                nameof(property));
        }

        propertyInfo.SetValue(_searchAggregate, value);

        return this;
    }

    public SearchAggregate Build() => _searchAggregate;

    public static SearchAggregateBuilder Create() => new();

    private static string GenerateUniqueProviderId() => Interlocked.Increment(ref _provierIdCounter).ToString();
}
