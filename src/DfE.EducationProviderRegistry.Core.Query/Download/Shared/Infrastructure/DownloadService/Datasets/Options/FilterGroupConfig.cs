namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Datasets.Options;

public class FilterGroupConfig
{
    public string GroupOperator { get; set; } = "And";
    public List<FilterConditionConfig> Conditions { get; set; } = [];
}
