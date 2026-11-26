using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;


internal sealed class GroupGovernorAggregateToTrusteesMapper :
    IMapper<IEnumerable<GroupGovernorAggregate>, IReadOnlyCollection<Trustee>>
{
    public IReadOnlyCollection<Trustee> Map(
        IEnumerable<GroupGovernorAggregate> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input
            .Where(governor => governor.Role != "Member")
            .OrderBy(governor => governor.RoleOrder)
            .Select(governor => new Trustee(
                id: new GovernanceIdentifier(governor.GovernorId),
                name: GroupGovernorAggregateNameMapper.CreateName(governor),
                role: governor.Role,
                startDate: governor.AppointmentStartDate,
                endDate: governor.AppointmentEndDate,
                title: governor.Title,
                appointedBy: governor.AppointingBody))
            .ToArray();
    }
}
