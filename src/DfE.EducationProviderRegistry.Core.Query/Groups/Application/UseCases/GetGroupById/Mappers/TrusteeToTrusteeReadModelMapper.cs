using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

internal sealed class TrusteeToTrusteeReadModelMapper : IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>
{
    public IReadOnlyCollection<TrusteeReadModel> Map(IEnumerable<Trustee> input)
    {
        return input?
            .OrderBy((trustee) => GetOrderPriority(trustee.Title?.Type))
            .ThenByDescending(trustee => trustee.StartDate)
            .Select(MapToReadModel)
            .ToArray() ?? [];
    }

    private static int GetOrderPriority(TrusteeTitleType? type) =>
        type switch
        {
            TrusteeTitleType.Chair => 0,
            TrusteeTitleType.CFO => 1,
            TrusteeTitleType.AccountingOfficer => 2,
            _ => 99
        };

    private static TrusteeReadModel MapToReadModel(Trustee trustee) => new()
    {
        Id = trustee.Id.Value,
        FullName = trustee.Name.Value,
        StartDate = trustee.StartDate,
        Title = trustee.Title?.Type
    };
}
