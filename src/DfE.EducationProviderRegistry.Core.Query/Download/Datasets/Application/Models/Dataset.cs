namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;

public sealed record Dataset(
    string Filename,
    string DataType,
    string DataFormat,
    int FileSize,
    byte[] File);
