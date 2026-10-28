namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Datasets.Options;

public class FilterConditionConfig
{
    public string Field { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Operator { get; set; } = "And";
}
