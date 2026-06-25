using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupExternalIdentifiers
{
    public GroupExternalIdentifiers(Ukprn? ukprn, CompaniesHouseId? companiesHouseId)
    {
        Ukprn = ukprn;
        CompaniesHouseId = companiesHouseId;
    }

    public Ukprn? Ukprn { get; }
    public CompaniesHouseId? CompaniesHouseId { get; }
}
