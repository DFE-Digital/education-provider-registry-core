using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles;

internal static class EstablishmentGovernorAggregateTestDoubles
{
    public static EstablishmentGovernorAggregate Stub()
    {
        return new EstablishmentGovernorAggregate
        {
            EstablishmentUrn = "123456",
            GovernorId = "GOV001",
            GovernorName = "Joe Bloggs",
            StartDate = new DateOnly(2023, 1, 1)
        };
    }

    public static IReadOnlyCollection<EstablishmentGovernorAggregate> StubMany()
    {
        return
        [
            new EstablishmentGovernorAggregate
            {
                EstablishmentUrn = "123456",
                GovernorId = "GOV001",
                GovernorName = "Joe Bloggs",
                StartDate = new DateOnly(2023, 1, 1)
            },
            new EstablishmentGovernorAggregate
            {
                EstablishmentUrn = "123456",
                GovernorId = "GOV002",
                GovernorName = "Jane Smith",
                StartDate = new DateOnly(2024, 1, 1)
            }
        ];
    }
}
