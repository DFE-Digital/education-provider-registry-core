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
    protected const string SearchField = nameof(Establishment.Name);

    protected SearchBehaviourTestsBase(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected abstract void ConfigureBehaviour(IndexedFieldConfigurationBuilder builder);

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

        builder.AddSearchConfiguration(
            (
                termKey: "term-1",
                fieldsConfigure:
                [
                    fieldBuilder =>
                    {
                        fieldBuilder.WithFieldName(SearchField);
                        ConfigureBehaviour(fieldBuilder);
                        fieldBuilder.Build();
                    }
                ]
            ));
    }

    protected async Task AssertExecutedSearchAsync(
        string searchTerm,
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

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm("term-1", searchTerm)
                .Build();

        // act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);

        Assert.NotNull(response.Model);
        Assert.Equal(
            matchSearchTerm.Count,
            response.Model.TotalNumberOfResults);

        Assert.NotNull(response.Model.EstablishmentResults);

        Assert.Equal(
            matchSearchTerm.Count,
            response.Model.EstablishmentResults.EstablishmentCollection.Count);

        List<EstablishmentSearchResult> results =
            response.Model.EstablishmentResults.EstablishmentCollection.ToList();

        for (int index = 0; index < results.Count; index++)
        {
            EstablishmentSearchResult establishmentResponse =
                results[index];

            Establishment seededEstablishment =
                searchedEstablishments.Establishments.Single(
                    t => t.Urn == establishmentResponse.Urn.Value);

            SearchResponseAssertions.AssertMapped(
                expected: seededEstablishment,
                actual: establishmentResponse);
        }
    }
}
