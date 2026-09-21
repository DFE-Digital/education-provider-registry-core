namespace DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.Model;

public sealed record Dataset(
    string Filename,
    string DataType,
    string DataFormat,
    int FileSize,
    byte[] File);
