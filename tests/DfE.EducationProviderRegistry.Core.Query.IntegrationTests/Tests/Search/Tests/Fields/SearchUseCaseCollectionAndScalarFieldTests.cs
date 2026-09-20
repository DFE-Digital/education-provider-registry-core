using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration.SearchConfiguration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseCollectionAndScalarFieldTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    private readonly string CollectionFieldName = string.Empty; // does not exist

    public SearchUseCaseCollectionAndScalarFieldTests(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            [
                (field) =>
                    field
                        .WithFieldName(DefaultSearchFieldName)
                        .AppendExactMatchBehaviour(),

                (field) =>
                    field
                        .WithFieldName(CollectionFieldName)
                        .AppendContainsMatchBehaviour()
            ],
            ChainFieldsBehaviour.AND);
    }

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Returns_Intersection_Of_Scalar_And_Collection_Field_Matches()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .WithLocalAuthorityName("School Authority")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .WithLocalAuthorityName("College Authority")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "academy")
                .WithLocalAuthorityName("School Authority")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
