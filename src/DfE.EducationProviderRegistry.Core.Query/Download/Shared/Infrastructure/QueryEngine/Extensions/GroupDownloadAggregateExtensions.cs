using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.Extensions;

[ExcludeFromCodeCoverage]
public static class GroupDownloadAggregateExtensions
{
    public static Expression<Func<GroupDownloadAggregate, GroupDownloadDto>> ToDto => ga => new GroupDownloadDto
    {
        GroupUniqueIdentifierUID = ga.GroupUid.HasValue ? ga.GroupUid.Value.ToString() : null,
        GroupId = ga.GroupId,
        GroupName = ga.GroupName,
        GroupTypeID = ga.GroupTypeId.HasValue ? ga.GroupTypeId.Value.ToString() : null,
        GroupType = ga.GroupType,
        GroupStatusID = ga.GroupStatusId.HasValue ? ga.GroupStatusId.Value.ToString() : null,
        GroupStatus = ga.GroupStatus,
        OpenDate = ga.OpenDate.ToString(),
        ClosedDate = ga.ClosedDate.ToString(),
        IncorporatedOn = ga.IncorporatedOn.ToString(),
        CompaniesHouseNumber = ga.CompaniesHouseNumber,
        UKProviderReferenceNumber = ga.UkProviderReferenceNumber,
        HeadOfGroupTitle = ga.HeadOfGroupTitle,
        HeadOfGroupFirstName = ga.HeadOfGroupFirstName,
        HeadOfGroupLastName = ga.HeadOfGroupLastName,
        GroupAddressLine1 = ga.GroupAddressLine1,
        GroupAddressLine2 = ga.GroupAddressLine2,
        GroupAddressLine3 = ga.GroupAddressLine3,
        GroupTownOrCity = ga.GroupTownOrCity,
        GroupCounty = ga.GroupCounty,
        GroupPostcode = ga.GroupPostcode
    };
}
