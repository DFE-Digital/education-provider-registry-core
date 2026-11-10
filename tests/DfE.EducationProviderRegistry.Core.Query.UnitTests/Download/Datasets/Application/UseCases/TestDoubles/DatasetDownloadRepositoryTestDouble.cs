using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Datasets.Application.UseCases.TestDoubles;

public static class DatasetDownloadRepositoryTestDouble
{
    public sealed class Capture
    {
        public string? DatasetName { get; internal set; }
        public string[]? FileNameKeys { get; internal set; }
    }

    public static Mock<IDatasetDownloadRepository> Mock() => new();

    public static Mock<IDatasetDownloadRepository> For(Dataset? result)
    {
        Mock<IDatasetDownloadRepository> mock = Mock();

        mock.Setup(downloadDatasetRepository =>
            downloadDatasetRepository.GetDatasetByName(
                It.IsAny<string>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    public static Mock<IDatasetDownloadRepository> ForCapturing(Dataset? result, Capture capture)
    {
        Mock<IDatasetDownloadRepository> mock = Mock();

        mock.Setup(downloadDatasetRepository =>
            downloadDatasetRepository.GetDatasetByName(
                It.IsAny<string>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, string[], CancellationToken>((datasetName, fileNameKeys, _) =>
            {
                capture.DatasetName = datasetName;
                capture.FileNameKeys = fileNameKeys;
            })
            .ReturnsAsync(result);

        return mock;
    }

    public static Mock<IDatasetDownloadRepository> Throwing(Exception ex)
    {
        Mock<IDatasetDownloadRepository> mock = Mock();

        mock.Setup(downloadDatasetRepository =>
            downloadDatasetRepository.GetDatasetByName(
                It.IsAny<string>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        return mock;
    }
}
