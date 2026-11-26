using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;

internal sealed class TrusteeToTrusteeReadModelMapper :
    IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>
{
    public IReadOnlyCollection<TrusteeReadModel> Map(IEnumerable<Trustee> input)
    {
        return input?
            .Select(MapToReadModel)
            .ToArray() ?? [];
    }

    private static TrusteeReadModel MapToReadModel(Trustee trustee) => new()
    {
        Id = trustee.Id.Value,
        FullName = trustee.Name.Value,
        Title = trustee.Title,
        Role = trustee.Role,
        AppointedBy = trustee.AppointedBy,
        StartDate = trustee.StartDate,
        EndDate = trustee.EndDate
    };
}
