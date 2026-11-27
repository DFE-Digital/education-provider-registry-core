using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Establishments;

public static class EstablishmentGovernorAggregateTestDoubles
{
    public static EstablishmentGovernorAggregate Stub()
    {
        return new EstablishmentGovernorAggregate
        {
            EstablishmentUrn = 123456,
            GovernorId = "GOV001",
            Title = "Mr",
            FirstName = "Joe",
            LastName = "Bloggs",
            Role = "Member",
            RoleOrder = 50,
            AppointmentStartDate = new DateOnly(2023, 1, 1),
            AppointmentEndDate = null
        };
    }

    public static IReadOnlyCollection<EstablishmentGovernorAggregate> StubMany()
    {
        return
        [
            new EstablishmentGovernorAggregate
            {
                EstablishmentUrn = 123456,
                GovernorId = "GOV001",
                Title = "Mr",
                FirstName = "Joe",
                LastName = "Bloggs",
                Role = "Member",
                RoleOrder = 50,
                AppointmentStartDate = new DateOnly(2023, 1, 1),
                AppointmentEndDate = null
            },
            new EstablishmentGovernorAggregate
            {
                EstablishmentUrn = 123456,
                GovernorId = "GOV002",
                Title = "Mrs",
                FirstName = "Jane",
                LastName = "Smith",
                Role = "Member",
                RoleOrder = 50,
                AppointmentStartDate = new DateOnly(2023, 1, 1),
                AppointmentEndDate = null
            }
        ];
    }
}
