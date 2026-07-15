using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;

internal static class SearchResponseAssertions
{
    public static void AssertMatches(
        Establishment expected,
        EstablishmentSearchResult actual)
    {
        Assert.Equal(
            expected.Urn,
            actual.Urn.Value);

        Assert.Equal(
            expected.Name,
            actual.Name.Value);

        Assert.Equal(
            expected.EstablishmentType?.Name,
            actual.Type?.Value);
    }
}
