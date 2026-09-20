using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests;

public abstract class SearchUseCaseMatchesResultsTestBase : UseCaseIntegrationTestBase
{
    // ensure fields do not have Unique Key constraints
    protected const string DefaultSearchFieldName = nameof(SearchAggregate.ProviderName);
    protected const string SecondarySearchFieldName = nameof(SearchAggregate.Postcode);

    protected SearchUseCaseMatchesResultsTestBase(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected virtual void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder) { }

    protected sealed override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSearch(configuration);
    }

    protected sealed override void ConfigureApplicationConfiguration(IConfigurationBuilder builder)
    {
        builder.AddSearchUseCaseConfiguration(ConfigureSearchUseCase);
    }

    protected async Task<UseCaseResponse<SearchResponse>> ExecuteAndAssertSearchAsync(
        SearchRequest request,
        IReadOnlyCollection<SearchAggregate> expectednResults,
        IReadOnlyCollection<SearchAggregate> notExpectedInResults)
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyCollection<SearchAggregate> seed = [.. expectednResults, .. notExpectedInResults];

        SearchableAggregates searchedAggregates =
            await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        // act

        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);

        Assert.NotNull(response.Model);
        Assert.Equal(expectednResults.Count, response.Model.TotalNumberOfResults);
        Assert.NotNull(response.Model.SearchProviderResults);
        Assert.Equal(expectednResults.Count, response.Model.SearchProviderResults.SearchResultCollection.Count);

        List<SearchAggregateResult> results = [.. response.Model.SearchProviderResults.SearchResultCollection];

        HashSet<string> resultUrns = [.. results.Select(t => t.UniqueIdentifier.Value)];

        Assert.DoesNotContain(notExpectedInResults, searchAggregate => resultUrns.Contains(searchAggregate.ProviderId!));

        for (int index = 0; index < results.Count; index++)
        {
            SearchAggregateResult searchAggregate = results[index];

            SearchAggregate seededAggregate =
                searchedAggregates.SearchAggregates.Single(
                    (t) => t.ProviderId == searchAggregate.UniqueIdentifier.Value);

            SearchResponseAssertions.AssertMapped(
                expected: seededAggregate,
                actual: searchAggregate);
        }

        return response;
    }
}
