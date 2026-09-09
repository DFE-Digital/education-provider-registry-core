using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentSearchResultsTestDouble
{
    public static SearchProviderResults Stub()
    {
        List<EstablishmentSearchResult> establishmentSearchResults = [];

        for (int i = 0; i < new Bogus.Faker().Random.Int(1, 10); i++)
        {
            establishmentSearchResults.Add(
                EstablishmentSearchResultTestDouble.Fake()); // Generate synthetic establishment search results instance
        }

        return new EstablishmentSearchResults(establishmentSearchResults);
    }

    public static SearchProviderResults EmptyStub() => new(null!);
}
