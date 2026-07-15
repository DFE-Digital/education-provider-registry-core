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

    public SearchEstablishmentBuilder WithName(string value)
    {
        _establishment.Name = value;
        return this;
    }

    public SearchEstablishmentBuilder WithUrn(string value)
    {
        _establishment.Urn = value;
        return this;
    }

    public Establishment Build() => _establishment;

    public static SearchEstablishmentBuilder Create() => new();
}
