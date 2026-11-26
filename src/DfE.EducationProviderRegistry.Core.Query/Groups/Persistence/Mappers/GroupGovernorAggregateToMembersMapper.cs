using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;

internal sealed class GroupGovernorAggregateToMembersMapper :
    IMapper<IEnumerable<GroupGovernorAggregate>, IReadOnlyCollection<Member>>
{
    public IReadOnlyCollection<Member> Map(
        IEnumerable<GroupGovernorAggregate> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input
            .Where(governor => governor.Role == "Member")
            .OrderBy(governor => governor.RoleOrder)
            .Select(governor => new Member(
                id: new GovernanceIdentifier(governor.GovernorId),
                name: GroupGovernorAggregateNameMapper.CreateName(governor),
                startDate: governor.AppointmentStartDate,
                endDate: governor.AppointmentEndDate,
                title: governor.Title,
                appointedBy: governor.AppointingBody))
            .ToArray();
    }
}
