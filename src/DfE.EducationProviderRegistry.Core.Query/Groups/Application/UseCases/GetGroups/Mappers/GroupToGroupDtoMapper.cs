using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroups.Mapper;

internal sealed class GroupToGroupDtoMapper : IMapper<Group, GroupDto>
{
    private readonly IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>> _trusteeToDtoMapper;
    private readonly IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberDto>> _memberToDtoMapper;

    public GroupToGroupDtoMapper(
        IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberDto>> memberToDtoMapper,
        IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>> trusteeToDtoMapper)
    {
        ArgumentNullException.ThrowIfNull(memberToDtoMapper);
        ArgumentNullException.ThrowIfNull(trusteeToDtoMapper);
        _trusteeToDtoMapper = trusteeToDtoMapper;
        _memberToDtoMapper = memberToDtoMapper;
    }

    public GroupDto Map(Group input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new GroupDto
        {
            GroupId = input.GroupId.Value,
            GroupUID = input.GroupUID.Value,
            CompaniesHouseId = input.CompaniesHouseId.Value,
            Academies = input.Academies.OrderBy(t => t.Name).ToArray(),
            Members = _memberToDtoMapper.Map(input.Members),
            Trustees = _trusteeToDtoMapper.Map(input.Trustees)
        };
    }
}
