using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

public sealed class SearchEstablishmentByNameReturnsResults : UseCaseIntegrationTestBase
{
    public SearchEstablishmentByNameReturnsResults(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration) => services.AddSearch(configuration);

    protected override void ConfigureApplicationConfiguration(IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            {
                "FilterKeyToFilterExpressionMapOptions:FilterChainingLogicalOperator",
                "AndLogicalOperator"
            },
            {
                "FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap:t.establishment_type_id:FilterExpressionKey",
                "SingleOrMultiValueEqualsExpression"
            },
            {
                "SearchCriteria:SearchFields:0",
                "Name"
            },
            {
                "SearchCriteria:Facets:0",
                "Name"
            }
        });
    }

    [Fact]
    public async Task Returns_Results()
    {
        CreatedEstablishmentResult created =
            await EstablishmentFactory.CreateAsync(
                (establishment) =>
                    establishment
                        .WithName("TEST")
                        .WithUrn("1111111"));

        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(
                new SearchRequest(
                    searchIndexKey: "STUB_SEARCH_INDEX_KEY",
                    searchKeywords: "TEST",
                    new SortOrder(sortField: "UNDEFINED", "asc",
                    validSortFields: ["UNDEFINED"])));

        Assert.NotNull(response);
    }
}
