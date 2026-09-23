using System.Diagnostics.CodeAnalysis;
using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Infrastructure;

[ExcludeFromCodeCoverage]
public sealed class FakeDatasetDownloadRepository : IDatasetDownloadRepository
{
    private readonly IFileCompressor _fileCompressor;

    public FakeDatasetDownloadRepository(IFileCompressor fileCompressor)
    {
        _fileCompressor = fileCompressor;
    }

    public async Task<Dataset?> GetDatasetByName(
        string datasetName,
        CancellationToken cancellationToken = default)
    {
        byte[] fileBytes = await FakeFileDownload();
        byte[] compressedFileBytes =
            _fileCompressor.CompressFile(("all-establishment-data.csv", fileBytes));

        return new Dataset(
            Filename: "all-establishment-data",
            DataType: "All Establishments",
            DataFormat: "CSV",
            FileSize: fileBytes.Length,
            File: compressedFileBytes);
    }

    public static Task<byte[]> FakeFileDownload()
    {
        StringBuilder csv = new();

        csv.AppendLine("UPN,Forename,Surname");
        csv.AppendLine("123456789012,John,Smith");
        csv.AppendLine("987654321098,Sarah,Jones");
        csv.AppendLine("555555555555,Michael,Brown");

        byte[] bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return Task.FromResult(bytes);
    }
}
