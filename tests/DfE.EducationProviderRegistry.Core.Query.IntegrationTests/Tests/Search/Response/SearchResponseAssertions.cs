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
            expected.EstablishmentType?.Name,
            actual.Type?.Value);

        // Site / Address
        Site? expectedSite = expected.Site.FirstOrDefault();

        //Assert.Equal(
        //    expectedSite?.AddressLine1,
        //    actual.Address?.Street);

        //Assert.Equal(
        //    expectedSite?.Town,
        //    actual.Address?.Town);

        //Assert.Equal(
        //    expectedSite?.County,
        //    actual.Address?.County);

        //Assert.Equal(
        //    expectedSite?.Postcode,
        //    actual.Address?.Postcode);

        // Local Authority
        //EstablishmentAuthority? expectedAuthority =
        //    expected.EstablishmentAuthority.FirstOrDefault();

        //Assert.Equal(
        //    expectedAuthority?.AuthorityName,
        //    actual.LocalAuthority?.Name);

        //Assert.Equal(
        //    expectedAuthority?.AuthorityCode,
        //    actual.LocalAuthority?.Code);

        //// Establishment > Group
        //EstablishmentGroupMembership? membership =
        //    expected.EstablishmentGroupMembership
        //        .FirstOrDefault();

        //Assert.Equal(
        //    membership?.Group?.Name,
        //    actual.Group?.PartOfName);

        //Assert.Equal(
        //    membership?.Group?.Code,
        //    actual.Group?.PartOfCode);
    }
}
