using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;

internal sealed class GroupToGroupReadModelMapper : IMapper<Group, GroupReadModel>
{
    private readonly IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>> _trusteeToDtoMapper;
    private readonly IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>> _memberToReadModelMapper;
    private readonly IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>> _establishmentToReadModelMapper;

    public GroupToGroupReadModelMapper(
        IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>> memberToDtoMapper,
        IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>> trusteeToDtoMapper,
        IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>> establishmentToReadModelMapper)
    {
        ArgumentNullException.ThrowIfNull(memberToDtoMapper);
        ArgumentNullException.ThrowIfNull(trusteeToDtoMapper);
        ArgumentNullException.ThrowIfNull(establishmentToReadModelMapper);
        _trusteeToDtoMapper = trusteeToDtoMapper;
        _memberToReadModelMapper = memberToDtoMapper;
        _establishmentToReadModelMapper = establishmentToReadModelMapper;
    }

    public GroupReadModel Map(Group input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new GroupReadModel
        {
            Name = input.Name.Value,
            GroupId = input.GroupId?.Value,
            GroupUID = input.GroupUID.Value,
            UKPRN = input.Ukprn?.Value,
            CompaniesHouseId = input.CompaniesHouseId?.Value,
            Address = input.Address.ToString(),
            Status = DisplayStatus(input.Status),
            StatusLabel = input.Status?.Label,
            OpenedDate = input.Status?.OpenedDate,
            ClosedDate = input.Status?.ClosedDate,
            TypeName = input.GroupType.Name,
            TypeCode = input.GroupType.Code.Value,
            Establishments = _establishmentToReadModelMapper.Map(input.Establishments),
            Members = _memberToReadModelMapper.Map(input.Members),
            Trustees = _trusteeToDtoMapper.Map(input.Trustees)
        };
    }

    private static string DisplayStatus(GroupStatus? status) =>
        status is null ?
            string.Empty :
                $"{status.Label} on {status.EffectiveDate.ToString("d MMMM yyyy")}";
}
