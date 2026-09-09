using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentTestDouble
{
    public static UniqueReferenceNumber ValidUrn => new("12345");

    public static Name ValidName => new("Test School");

    public static SearchProviderAddress ValidAddress =>
        new(
            Name: string.Empty,
            AddressLine1: "123 Example Street",
            AddressLine2: string.Empty,
            Town: "Testville",
            County: "Testshire",
            Postcode: "TE5 7ST");

    public static SearchProviderType ValidType =>
        SearchProviderType.Create("Academy");

    public static GroupDetail ValidGroup =>
        GroupDetail.Create("Mock Trust", "TRUST001");

    public static SearchProviderLocalAuthority ValidLocalAuthority =>
        SearchProviderLocalAuthority.Create("Test LA", "LA001");
}

