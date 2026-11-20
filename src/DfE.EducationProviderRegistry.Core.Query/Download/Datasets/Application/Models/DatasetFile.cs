namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;

public sealed record DatasetFile(
    string Filename,
    string DataType,
    string DataFormat,
    long FileSize,
    Stream FileStream);
