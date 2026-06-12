namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentAddress
{
    public string Street { get; }
    public string Locality { get; }
    public string Town { get; }
    public string County { get; }
    public string Postcode { get; }

    public EstablishmentAddress(string street, string locality, string town, string county, string postcode)
    {
        ArgumentNullException.ThrowIfNull(street);
        ArgumentNullException.ThrowIfNull(locality);
        ArgumentNullException.ThrowIfNull(town);
        ArgumentNullException.ThrowIfNull(county);
        ArgumentNullException.ThrowIfNull(postcode);

        Street = street;
        Locality = locality;
        Town = town;
        County = county;
        Postcode = postcode;
    }
}




