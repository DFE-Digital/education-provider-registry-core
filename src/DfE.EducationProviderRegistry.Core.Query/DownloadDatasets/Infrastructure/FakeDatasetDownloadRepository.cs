using System.Text;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Infrastructure;

public sealed class FakeDatasetDownloadRepository : IDatasetDownloadRepository
{
    public async Task<Dataset?> GetDatasetByName(
        string datasetName,
        CancellationToken cancellationToken = default)
    {
        byte[] fileBytes = await FakeFileDownload();

        return new Dataset(
            Filename: "all-establishment-data",
            DataType: "All Establishments",
            DataFormat: "CSV",
            FileSize: fileBytes.Length,
            File: fileBytes);
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
