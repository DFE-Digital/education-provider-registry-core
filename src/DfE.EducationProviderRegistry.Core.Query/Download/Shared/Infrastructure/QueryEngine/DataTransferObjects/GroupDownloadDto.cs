namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;

public sealed class GroupDownloadDto
{
    public string? GroupUniqueIdentifierUID { get; set; }
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
    public string? GroupType { get; set; }
}
