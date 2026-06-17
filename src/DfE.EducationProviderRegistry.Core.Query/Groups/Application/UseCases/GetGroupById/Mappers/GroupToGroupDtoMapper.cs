using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

internal sealed class GroupToGroupDtoMapper : IMapper<Group, GroupDto>
{
    private readonly IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>> _trusteeToDtoMapper;
    private readonly IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>> _memberToReadModelMapper;

    public GroupToGroupDtoMapper(
        IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>> memberToDtoMapper,
        IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>> trusteeToDtoMapper)
    {
        ArgumentNullException.ThrowIfNull(memberToDtoMapper);
        ArgumentNullException.ThrowIfNull(trusteeToDtoMapper);
        _trusteeToDtoMapper = trusteeToDtoMapper;
        _memberToReadModelMapper = memberToDtoMapper;
    }

    public GroupDto Map(Group input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new GroupDto
        {
            GroupId = input.GroupId.Value,
            GroupUID = input.GroupUID.Value,
            CompaniesHouseId = input.CompaniesHouseId.Value,
            Academies = input.Academies.OrderBy(t => t.Name.ToString()).ToArray(),
            Members = _memberToReadModelMapper.Map(input.Members),
            Trustees = _trusteeToDtoMapper.Map(input.Trustees)
        };
    }
}
