namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Datasets.Options;

public class DownloadOptions
{
    public Dictionary<string, DownloadProfileConfig> DownloadProfiles { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}
