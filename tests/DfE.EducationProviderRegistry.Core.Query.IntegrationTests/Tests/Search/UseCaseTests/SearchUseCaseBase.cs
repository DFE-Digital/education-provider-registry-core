using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests;

public abstract class SearchUseCaseBase : UseCaseIntegrationTestBase
{
    // ensure fields do not have UK constraints
    protected const string DefaultSearchFieldName = nameof(SearchAggregate.ProviderName);
    protected const string SecondarySearchFieldName = nameof(SearchAggregate.Postcode);

    protected SearchUseCaseBase(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected abstract (string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration();

    protected virtual IEnumerable<KeyValuePair<string, string?>> CreateFilterExpressionOptions() => FilterKeyToFilterExpressionMapOptionsStub.StubFilter;

    protected override async Task AfterStartTestDependenciesAsync(CancellationToken ct = default)
    {
        // Clear all establishments and assoc to avoid conflicts with searchTerms
        await SearchAggregateFixture.ClearAsync(ct);
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
        builder.StubFilterOptions(filterKeyToFilterMapping: CreateFilterExpressionOptions());

        builder.StubSearchCriteriaOptions();

        builder.AddSearchConfiguration(CreateSearchTermsConfiguration());
    }

    protected async Task<UseCaseResponse<SearchResponse>> ExecuteAndAssertSearchAsync(
        SearchRequest request,
        IReadOnlyCollection<SearchAggregate> expectednResults,
        IReadOnlyCollection<SearchAggregate> notExpectedInResults)
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchableAggregates searchedAggregates =
            await SearchAggregateFixture.PersistAsync(
                [
                    .. expectednResults,
                    .. notExpectedInResults
                ],
                ct);

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
