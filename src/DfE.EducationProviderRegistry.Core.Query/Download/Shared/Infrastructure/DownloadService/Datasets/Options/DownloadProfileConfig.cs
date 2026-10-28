namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Datasets.Options;

public class DownloadProfileConfig
{
    public List<string> SelectedColumns { get; set; } = [];
    public List<FilterGroupConfig> Groups { get; set; } = [];
}
