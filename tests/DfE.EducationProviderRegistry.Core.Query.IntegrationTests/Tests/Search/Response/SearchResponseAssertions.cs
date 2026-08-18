using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;

internal static class SearchResponseAssertions
{
    public static void AssertMapped(
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
            expected.EstablishmentType?.Name ?? string.Empty,
            actual.Type?.Value ?? string.Empty);

        // Site / Address
        Site? expectedSite = expected.Site.FirstOrDefault();

        Assert.Equal(
            expectedSite?.AddressLine1 ?? string.Empty,
            actual.Address?.Street ?? string.Empty);

        Assert.Equal(
            expectedSite?.Town ?? string.Empty,
            actual.Address?.Town ?? string.Empty);

        Assert.Equal(
            expectedSite?.County ?? string.Empty,
            actual.Address?.County ?? string.Empty);

        Assert.Equal(
            expectedSite?.Postcode ?? string.Empty,
            actual.Address?.Postcode ?? string.Empty);

        // Local Authority
        EstablishmentAuthority? expectedAuthority =
            expected.EstablishmentAuthority.FirstOrDefault();

        Assert.Equal(
            expectedAuthority?.AuthorityName ?? string.Empty,
            actual.LocalAuthority?.Name ?? string.Empty);

        Assert.Equal(
            expectedAuthority?.AuthorityCode ?? string.Empty,
            actual.LocalAuthority?.Code ?? string.Empty);

        // Establishment > Group
        EstablishmentGroupMembership? membership =
            expected.EstablishmentGroupMembership
                .FirstOrDefault();

        Assert.Equal(
            membership?.Group?.Name ?? string.Empty,
            actual.Group?.PartOfName ?? string.Empty);

        Assert.Equal(
            membership?.Group?.Code ?? string.Empty,
            actual.Group?.PartOfCode ?? string.Empty);
    }
}
