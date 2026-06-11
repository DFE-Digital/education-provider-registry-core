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
            .Select((trustee) => new TrusteeDto
            {
                Id = trustee.Id.Value,
                FullName = trustee.Name.FullName,
                StartDate = trustee.StartDate,
                Title = trustee.Title?.Type
            })
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
}
