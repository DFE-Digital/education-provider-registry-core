using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

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
            Address = DisplayAddress(input.Address),
            Status = DisplayStatus(input.Status),
            Type = input.GroupType.Value,
            Academies = input.Academies.OrderBy(t => t.Name.ToString()).ToArray(),
            Members = _memberToReadModelMapper.Map(input.Members),
            Trustees = _trusteeToDtoMapper.Map(input.Trustees)
        };
    }

    private static string DisplayAddress(Address address) => $"{address.Street}, {address.Town}, {address.County}, {address.Postcode}";

    private static string DisplayStatus(GroupStatus status)
        => $"{(status.State == GroupOpenState.Open ? "Opened" : "Closed")} on {status.EffectiveDate.ToString("d MMMM yyyy")}";
}
