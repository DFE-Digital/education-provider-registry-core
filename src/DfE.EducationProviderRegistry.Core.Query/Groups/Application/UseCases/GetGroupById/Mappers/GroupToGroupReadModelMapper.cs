using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

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
            Name = input.Name.Value,
            GroupId = input.GroupId.Value,
            GroupUID = input.GroupUID.Value,
            UKPRN = input.Ukprn.Value,
            CompaniesHouseId = input.CompaniesHouseId?.Value,
            Address = input.Address.ToString(),
            Status = DisplayStatus(input.Status),
            Type = input.GroupType.Value,
            Academies = input.Academies.OrderBy(t => t.Name.ToString()).ToArray(),
            Members = _memberToReadModelMapper.Map(input.Members),
            Trustees = _trusteeToDtoMapper.Map(input.Trustees)
        };
    }

    private static string DisplayStatus(GroupStatus? status) =>
        status is null ?
            string.Empty :
                $"{status.Label} on {status.EffectiveDate.ToString("d MMMM yyyy")}";
}
