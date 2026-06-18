using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;

internal sealed class GroupToGroupReadModelMapper : IMapper<Group, GroupReadModel>
{
    private readonly IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>> _trusteeToDtoMapper;
    private readonly IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>> _memberToReadModelMapper;

    public GroupToGroupReadModelMapper(
        IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>> memberToDtoMapper,
        IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>> trusteeToDtoMapper)
    {
        ArgumentNullException.ThrowIfNull(memberToDtoMapper);
        ArgumentNullException.ThrowIfNull(trusteeToDtoMapper);
        _trusteeToDtoMapper = trusteeToDtoMapper;
        _memberToReadModelMapper = memberToDtoMapper;
    }

    public GroupReadModel Map(Group input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new GroupReadModel
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
