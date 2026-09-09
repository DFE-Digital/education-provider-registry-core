using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles.Builders;

[ExcludeFromCodeCoverage]
internal sealed class EstablishmentSearchResultBuilder
{
    private UniqueReferenceNumber _urn = new("12345");
    private Name _name = new("Test School");
    private SearchProviderAddress _address = new(
        Name: string.Empty,
        AddressLine1: "123 Example Street",
        AddressLine2: string.Empty,
        Town: "Testville",
        County: "Testshire",
        Postcode: "TE5 7ST");
    private SearchProviderType _type = SearchProviderType.Create("Academy");
    private GroupDetail _group = GroupDetail.Create("Mock Trust", "TRUST001");
    private SearchProviderLocalAuthority _localAuthority = SearchProviderLocalAuthority.Create("Test LA", "LA001");

    public EstablishmentSearchResultBuilder WithUrn(string urn)
    {
        _urn = new ProviderIdentifier(urn);
        return this;
    }

    public EstablishmentSearchResultBuilder WithName(string name)
    {
        _name = new Name(name);
        return this;
    }

    public EstablishmentSearchResultBuilder WithAddress(SearchProviderAddress address)
    {
        _address = address;
        return this;
    }

    public EstablishmentSearchResultBuilder WithType(string type)
    {
        _type = SearchProviderType.Create(type);
        return this;
    }

    public EstablishmentSearchResultBuilder WithGroup(string name, string code)
    {
        _group = GroupDetail.Create(name, code);
        return this;
    }

    public EstablishmentSearchResultBuilder WithLocalAuthority(string name, string code)
    {
        _localAuthority = SearchProviderLocalAuthority.Create(name, code);
        return this;
    }

    public EstablishmentSearchResult Build() =>
        EstablishmentSearchResult.Create(
            _urn,
            _name,
            _address,
            _type,
            _group,
            _localAuthority);
}
