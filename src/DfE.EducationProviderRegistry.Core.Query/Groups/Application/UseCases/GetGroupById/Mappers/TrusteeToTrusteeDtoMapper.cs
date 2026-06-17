using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

internal sealed class TrusteeToTrusteeDtoMapper : IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>>
{
    public IReadOnlyCollection<TrusteeDto> Map(IEnumerable<Trustee> input)
    {
        return input?
            .OrderBy((trustee) => GetOrderPriority(trustee.Title?.Type))
            .ThenByDescending(trustee => trustee.StartDate)
            .Select(MapToDto)
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

    private static TrusteeDto MapToDto(Trustee trustee) => new()
    {
        Id = trustee.Id.Value,
        FullName = trustee.Name.Value,
        StartDate = trustee.StartDate,
        Title = trustee.Title?.Type
    };
}
