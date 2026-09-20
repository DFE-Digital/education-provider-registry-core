using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Sort;

public sealed class SearchUseCaseSortTests : UseCaseIntegrationTestBase
{
    private static readonly SearchAggregate[] SortableSearchResults =
    [
        SearchAggregateBuilder.Create().WithProviderName("school-3").Build(),
        SearchAggregateBuilder.Create().WithProviderName("school-1").Build(),
        SearchAggregateBuilder.Create().WithProviderName("school-2").Build(),
    ];

    public SearchUseCaseSortTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected sealed override void ConfigureApplicationServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSearch(configuration);
    }

    protected sealed override void ConfigureApplicationConfiguration(
        IConfigurationBuilder builder)
    {
        builder.AddSearchUseCaseConfiguration((configureUseCase) =>
            configureUseCase.WithSearchTerm(
                "term-1",
                (field) =>
                    field.WithFieldName(nameof(SearchAggregate.ProviderName))
                        .AppendContainsMatchBehaviour()));
    }

    [Fact]
    public async Task Returns_Ascending_Sort_ByName()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchableAggregates _ =
            await DatabaseFixture.SeedAsync<
                IEnumerable<SearchAggregate>, SearchableAggregates>(SortableSearchResults, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm("term-1", "school")
                .WithSortDirection("asc")
                .Build();

        // Act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request, ct);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Model);
        Assert.NotNull(response.Model.SearchProviderResults);

        Assert.Collection(
            response.Model.SearchProviderResults.SearchResultCollection,
            (current) => SearchResponseAssertions.AssertMapped(SortableSearchResults.Single(t => t.ProviderName == "school-1"), current),
            (current) => SearchResponseAssertions.AssertMapped(SortableSearchResults.Single(t => t.ProviderName == "school-2"), current),
            (current) => SearchResponseAssertions.AssertMapped(SortableSearchResults.Single(t => t.ProviderName == "school-3"), current));
    }

    [Fact]
    public async Task Returns_Descending_Sort_ByName()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchableAggregates _ =
            await DatabaseFixture.SeedAsync<
                IEnumerable<SearchAggregate>, SearchableAggregates>(SortableSearchResults, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm("term-1", "school")
                .WithSortDirection("desc")
                .Build();

        // Act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request, ct);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Model);
        Assert.NotNull(response.Model.SearchProviderResults);

        Assert.Collection(
            response.Model.SearchProviderResults.SearchResultCollection,
            (current) => SearchResponseAssertions.AssertMapped(SortableSearchResults.Single(t => t.ProviderName == "school-3"), current),
            (current) => SearchResponseAssertions.AssertMapped(SortableSearchResults.Single(t => t.ProviderName == "school-2"), current),
            (current) => SearchResponseAssertions.AssertMapped(SortableSearchResults.Single(t => t.ProviderName == "school-1"), current));
    }
}
