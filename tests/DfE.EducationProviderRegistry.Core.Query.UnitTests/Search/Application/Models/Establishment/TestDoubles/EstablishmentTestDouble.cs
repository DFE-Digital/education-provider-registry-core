using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentTestDouble
{
    public static UniqueReferenceNumber ValidUrn => new("12345");

    public static Name ValidName => new("Test School");

    public static Address ValidAddress =>
        new(
            Street: "123 Example Street",
            Town: "Testville",
            County: "Testshire",
            Postcode: "TE5 7ST");

    public static EstablishmentType ValidType =>
        EstablishmentType.Create("Academy");

    public static GroupDetail ValidGroup =>
        GroupDetail.Create("Mock Group Id", "Mock Trust", "TRUST001");

    public static LocalAuthority ValidLocalAuthority =>
        LocalAuthority.Create("Test LA", "LA001");
}

