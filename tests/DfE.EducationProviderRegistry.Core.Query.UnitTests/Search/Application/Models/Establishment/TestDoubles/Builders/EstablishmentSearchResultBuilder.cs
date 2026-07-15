using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles.Builders;

[ExcludeFromCodeCoverage]
internal sealed class EstablishmentSearchResultBuilder
{
    private UniqueReferenceNumber _urn = new("12345");
    private Name _name = new("Test School");
    private Address _address = new(
        Street: "123 Example Street",
        Town: "Testville",
        County: "Testshire",
        Postcode: "TE5 7ST");
    private EstablishmentType _type = EstablishmentType.Create("Academy");
    private GroupDetail _group = GroupDetail.Create("Mock Group Id", "Mock Trust", "TRUST001");
    private LocalAuthority _localAuthority = LocalAuthority.Create("Test LA", "LA001");

    public EstablishmentSearchResultBuilder WithUrn(string urn)
    {
        _urn = new UniqueReferenceNumber(urn);
        return this;
    }

    public EstablishmentSearchResultBuilder WithName(string name)
    {
        _name = new Name(name);
        return this;
    }

    public EstablishmentSearchResultBuilder WithAddress(Address address)
    {
        _address = address;
        return this;
    }

    public EstablishmentSearchResultBuilder WithType(string type)
    {
        _type = EstablishmentType.Create(type);
        return this;
    }

    public EstablishmentSearchResultBuilder WithGroup(string id, string name, string code)
    {
        _group = GroupDetail.Create(id, name, code);
        return this;
    }

    public EstablishmentSearchResultBuilder WithLocalAuthority(string name, string code)
    {
        _localAuthority = LocalAuthority.Create(name, code);
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
