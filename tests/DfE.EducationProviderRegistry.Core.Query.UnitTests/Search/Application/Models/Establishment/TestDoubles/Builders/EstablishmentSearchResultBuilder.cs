using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles.Builders;

[ExcludeFromCodeCoverage]
internal sealed class SearchResultBuilder
{
    private ProviderIdentifier _providerIdentifier = new("12345");
    private Name _name = new("Test School");
    private SearchAddress _address = new(
        "123 Example Street, " +
        "Testville, " +
        "Testshire, " +
        "TE5 7ST");
    private SearchType _type = SearchType.Create("Academy", 1);
    private GroupDetail _group = GroupDetail.Create("Mock Trust", "TRUST001");
    private SearchLocalAuthority _localAuthority = SearchLocalAuthority.Create("Test LA");
    private SearchCategory _providerCategory = new("Establishment");

    private int _academyCount = 10;

    public SearchResultBuilder WithUrn(string providerIdentifier)
    {
        _providerIdentifier = new ProviderIdentifier(providerIdentifier);
        return this;
    }

    public SearchResultBuilder WithName(string name)
    {
        _name = new Name(name);
        return this;
    }

    public SearchResultBuilder WithAddress(SearchAddress address)
    {
        _address = address;
        return this;
    }

    public SearchResultBuilder WithType(string type, long id)
    {
        _type = SearchType.Create(type, id);
        return this;
    }

    public SearchResultBuilder WithGroup(string name, string code)
    {
        _group = GroupDetail.Create(name, code);
        return this;
    }

    public SearchResultBuilder WithLocalAuthority(string name)
    {
        _localAuthority = SearchLocalAuthority.Create(name);
        return this;
    }

    public SearchResultBuilder WithProviderCategory(string category)
    {
        _providerCategory = SearchCategory.Create(category);
        return this;
    }

    public SearchResultBuilder WithAcademyCount(int academyCount)
    {
        _academyCount = academyCount;
        return this;
    }

    public SearchAggregateResult Build() =>
        SearchAggregateResult.Create(
            _providerIdentifier,
            _name,
            _address,
            _type,
            _group,
            _localAuthority,
            _providerCategory,
            _academyCount);
}
