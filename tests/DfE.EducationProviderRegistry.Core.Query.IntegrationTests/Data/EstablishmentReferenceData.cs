namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data;

// used for DB lookups to get the correct IDs for the establishment type, status, and headteacher role type
public sealed class EstablishmentReferenceData
{
    public string EstablishmentTypeCode { get; set; } = "PRI";
    public string EstablishmentStatusCode { get; set; } = "OPEN";
    public string HeadteacherRoleTypeCode { get; set; } = "HT";
}
