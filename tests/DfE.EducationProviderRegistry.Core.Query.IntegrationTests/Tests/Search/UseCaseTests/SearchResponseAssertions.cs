using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests;

internal static class SearchResponseAssertions
{
    public static void AssertMapped(
        SearchAggregate expected,
        SearchAggregateResult actual)
    {
        Assert.Equal(
            expected.ProviderId,
            actual.UniqueIdentifier.Value);

        Assert.Equal(
            expected.ProviderName,
            actual.Name.Value);

        Assert.Equal(
            expected.ProviderTypeName ?? string.Empty,
            actual.Type?.Name ?? string.Empty);

        // Site / Address
        Assert.Equal(
            expected.ProviderAddress ?? string.Empty,
            actual.Address?.FullAddress ?? string.Empty);

        // Local Authority
        Assert.Equal(
            expected.LocalAuthorityName ?? string.Empty,
            actual.LocalAuthority?.Name ?? string.Empty);
    }
}
