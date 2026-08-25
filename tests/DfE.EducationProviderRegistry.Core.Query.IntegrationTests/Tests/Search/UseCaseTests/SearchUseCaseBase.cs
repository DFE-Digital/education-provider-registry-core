using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests;

public abstract class SearchUseCaseBase : UseCaseIntegrationTestBase
{
    // ensure fields do not have UK constraints
    protected const string DefaultSearchFieldName = nameof(Establishment.Name);
    protected const string SecondarySearchFieldName = nameof(Establishment.EstablishmentNumber);
    protected const string CollectionFieldName = "EstablishmentAuthority[].AuthorityName";

    protected SearchUseCaseBase(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected abstract (string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration();

    protected virtual IEnumerable<KeyValuePair<string, string?>> CreateFilterExpressionOptions() => FilterKeyToFilterExpressionMapOptionsStub.StubFilter;

    protected override async Task AfterStartTestDependenciesAsync(CancellationToken ct = default)
    {
        // Clear all establishments and assoc to avoid conflicts with searchTerms
        await SeedSearchEstablishments.ClearAsync(ct);
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
        IReadOnlyCollection<Establishment> expectednResults,
        IReadOnlyCollection<Establishment> notExpectedInResults)
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchableEstablishments searchedEstablishments =
            await SeedSearchEstablishments.SeedAsync(
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
        Assert.NotNull(response.Model.EstablishmentResults);
        Assert.Equal(expectednResults.Count, response.Model.EstablishmentResults.EstablishmentCollection.Count);

        List<EstablishmentSearchResult> results = [.. response.Model.EstablishmentResults.EstablishmentCollection];

        HashSet<string> resultUrns = [.. results.Select(t => t.Urn.Value)];

        Assert.DoesNotContain(notExpectedInResults, establishment => resultUrns.Contains(establishment.Urn!));

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

        return response;
    }
}
