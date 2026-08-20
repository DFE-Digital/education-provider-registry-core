using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public abstract class SearchBehaviourTestsBase : UseCaseIntegrationTestBase
{
    // ensure fields do not have UK constraints
    protected const string DefaultSearchFieldName = nameof(Establishment.Name);
    protected const string SecondarySearchFieldName = nameof(Establishment.EstablishmentNumber);

    protected SearchBehaviourTestsBase(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected abstract (string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration();

    protected override async Task AfterStartTestDependenciesAsync(CancellationToken ct = default)
    {
        // Clear all establishments and assoc to avoid conflicts with searchTerms
        await SeedSearchEstablishments.ClearAsync(ct);
    }

    protected override void ConfigureApplicationServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSearch(configuration);
    }

    protected override void ConfigureApplicationConfiguration(
        IConfigurationBuilder builder)
    {
        builder
            .StubFilterOptions()
            .StubSearchCriteriaOptions();

        builder.AddSearchConfiguration(CreateSearchTermsConfiguration());
    }

    protected async Task AssertExecutedSearchAsync(
        IEnumerable<(string key, string value)> searchTerms,
        IReadOnlyCollection<Establishment> matchSearchTerm,
        IReadOnlyCollection<Establishment> nonMatchSearchTerm)
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchableEstablishments searchedEstablishments =
            await SeedSearchEstablishments.SeedAsync(
                [
                    .. matchSearchTerm,
                    .. nonMatchSearchTerm
                ],
                ct);

        SearchRequest request = BuildSearchRequest(searchTerms);

        // act

        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);

        Assert.NotNull(response.Model);
        Assert.Equal(matchSearchTerm.Count, response.Model.TotalNumberOfResults);
        Assert.NotNull(response.Model.EstablishmentResults);
        Assert.Equal(matchSearchTerm.Count, response.Model.EstablishmentResults.EstablishmentCollection.Count);

        List<EstablishmentSearchResult> results = [.. response.Model.EstablishmentResults.EstablishmentCollection];

        HashSet<string> resultUrns = [.. results.Select(t => t.Urn.Value)];
        Assert.DoesNotContain(nonMatchSearchTerm, establishment => resultUrns.Contains(establishment.Urn!));

        for (int index = 0; index < results.Count; index++)
        {
            EstablishmentSearchResult establishmentResponse = results[index];

            Establishment seededEstablishment =
                searchedEstablishments.Establishments.Single(
                    (t) => t.Urn == establishmentResponse.Urn.Value);

            SearchResponseAssertions.AssertMapped(
                expected: seededEstablishment,
                actual: establishmentResponse);
        }
    }

    private static SearchRequest BuildSearchRequest(IEnumerable<(string key, string value)> searchTerms)
    {
        SearchRequestBuilder requestBuilder = SearchRequestBuilder.Create();

        foreach ((string key, string value) termTuple in searchTerms)
        {
            requestBuilder.WithSearchTerm(termTuple);
        }

        return requestBuilder.Build();
    }
}
