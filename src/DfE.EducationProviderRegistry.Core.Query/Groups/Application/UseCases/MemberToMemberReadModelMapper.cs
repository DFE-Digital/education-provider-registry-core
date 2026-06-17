using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;

internal sealed class MemberToMemberReadModelMapper : IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>
{
    public IReadOnlyCollection<MemberReadModel> Map(IEnumerable<Member> input)
    {
        return input?
            .OrderByDescending(member => member.StartDate)
            .Select(MapToReadModel)
            .ToArray() ?? [];
    }

    private static MemberReadModel MapToReadModel(Member member) => new()
    {
        Identifier = member.Id.Value,
        FullName = member.Name.Value,
        StartDate = member.StartDate,
    };
}
