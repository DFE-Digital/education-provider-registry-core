using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class GroupGovernorAggregateTestDoubles
{
    public static GroupGovernorAggregate CreateWith(
        string governorId = "1234567",
        string? firstName = "Test",
        string lastName = "Governor",
        string role = "Trustee",
        int roleOrder = 40,
        string? title = "Mr",
        string? appointingBody = "Appointed by academy members",
        DateOnly? appointmentStartDate = null,
        DateOnly? appointmentEndDate = null)
    {
        return new()
        {
            GovernorId = governorId,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            RoleOrder = roleOrder,
            Title = title,
            AppointingBody = appointingBody,
            AppointmentStartDate =
                appointmentStartDate ?? new DateOnly(2024, 1, 1),
            AppointmentEndDate = appointmentEndDate
        };
    }
}
