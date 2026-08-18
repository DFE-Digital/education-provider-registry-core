using System.Linq.Expressions;
using System.Reflection;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

public sealed class SearchEstablishmentBuilder
{
    // temp as seed range 10_000->9_010_000 avoids conflicts
    private static int _urnCounter = 9_010_001;

    private readonly Establishment _establishment;

    public SearchEstablishmentBuilder()
    {
        _establishment = new Establishment
        {
            Name = "Test Establishment",
            Urn = Interlocked.Increment(ref _urnCounter).ToString()
        };

        _establishment.Site.Add(
            new Site
            {
                AddressLine1 = "1 Test Street",
                Town = "Test Town",
                County = "Test County",
                Postcode = "TE1 1ST"
            });

        _establishment.EstablishmentAuthority.Add(
            new EstablishmentAuthority
            {
                AuthorityCode = "001",
                AuthorityName = "Test Authority"
            });
    }

    public SearchEstablishmentBuilder SetValue(string property, string value)
    {
        if (string.IsNullOrWhiteSpace(property))
        {
            throw new ArgumentException(
                "Property cannot be null or whitespace.",
                nameof(property));
        }

        PropertyInfo? propertyInfo = _establishment.GetType().GetProperty(property);

        if (propertyInfo is null)
        {
            throw new ArgumentException(
                $"Property '{property}' does not exist on {nameof(Establishment)}.",
                nameof(property));
        }

        if (!propertyInfo.CanWrite)
        {
            throw new ArgumentException(
                $"Property '{property}' is read-only.",
                nameof(property));
        }

        propertyInfo.SetValue(_establishment, value);

        return this;
    }
    public Establishment Build() => _establishment;

    public static SearchEstablishmentBuilder Create() => new();
}
