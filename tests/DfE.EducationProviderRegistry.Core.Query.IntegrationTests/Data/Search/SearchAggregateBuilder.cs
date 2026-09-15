using System.Reflection;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

public sealed class SearchAggregateBuilder
{
    // temp as seed range 10_000->9_010_000 avoids conflicts
    private static int _providerIdentifierCounter = 9_010_001;

    private readonly SearchAggregate _searchAggregate;

    public SearchAggregateBuilder()
    {
        _searchAggregate = new SearchAggregate
        {
            ProviderId = Interlocked.Increment(ref _providerIdentifierCounter).ToString(),
            ProviderName = "Test Establishment",
            ProviderAddress = "1 Test Street, Test Town, Test County, TE1 1ST",
            Town = "Test Town",
            County = "Test County",
            Postcode = "TE1 1ST",
            LocalAuthorityName = "Test Authority"
        };

    }

    public SearchAggregateBuilder WithProviderId(string providerId)
    {
        _searchAggregate.ProviderId = providerId;
        return this;
    }

    public SearchAggregateBuilder WithAuthorityName(string localAuthorityName)
    {
        _searchAggregate.LocalAuthorityName = localAuthorityName;

        return this;
    }

    public SearchAggregateBuilder WithName(string name)
    {
        _searchAggregate.ProviderName = name;
        return this;
    }

    public SearchAggregateBuilder WithEstablishmentTypeId(long typeId)
    {
        _searchAggregate.ProviderTypeId = typeId;
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
}
