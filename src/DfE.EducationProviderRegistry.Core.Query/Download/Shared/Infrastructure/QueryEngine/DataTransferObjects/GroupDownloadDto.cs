namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;

public sealed record GroupDownloadDto
{
    public string? GroupUniqueIdentifierUID { get; init; }
    public string? GroupId { get; init; }
    public string? GroupName { get; init; }
    public string? GroupType { get; init; }
    public string? GroupTypeID { get; init; }
    public string? GroupStatusID { get; init; }
    public string? GroupStatus { get; init; }
    public string? OpenDate{ get; init; }
    public string? ClosedDate { get; init; }
    public string? IncorporatedOn { get; init; }
    public string? CompaniesHouseNumber { get; init; }
    public string? UKProviderReferenceNumber { get; init; }
    public string? HeadOfGroupName { get; init; }
    public string? GroupAddressLine1 { get; init; }
    public string? GroupAddressLine2 { get; init; }
    public string? GroupAddressLine3 { get; init; }
    public string? GroupTownOrCity { get; init; }
    public string? GroupCounty { get; init; }
    public string? GroupPostcode { get; init; }
}
