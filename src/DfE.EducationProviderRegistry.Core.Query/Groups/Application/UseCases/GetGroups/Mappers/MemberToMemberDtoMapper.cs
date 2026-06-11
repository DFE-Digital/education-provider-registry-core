using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups.Mappers;

internal sealed class MemberToMemberDtoMapper : IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberDto>>
{
    public IReadOnlyCollection<MemberDto> Map(IEnumerable<Member> input)
    {
        return input?
            .OrderByDescending(member => member.StartDate)
            .Select((member) => new MemberDto
            {
                Identifier = member.Id.Value,
                FullName = member.Name.FullName,
                StartDate = member.StartDate,
            })
            .ToArray() ?? [];
    }
}
