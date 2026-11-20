using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv.TestDoubles;

public static class PostgresCsvReaderTestDouble
{
    public static Mock<IDownloadExporter> Mock() => new(MockBehavior.Strict);

    public static FakePostgresCsvReader CreateFake(string csv) => new(csv);

    public static Mock<IDownloadExporter> MockFor(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        Mock<IDownloadExporter> mock =
            new(MockBehavior.Strict);

        mock
            .Setup(reader => reader.ExportToStreamAsync(
                It.IsAny<Stream>(),
                It.IsAny<IQueryable<object>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        return mock;
    }
}
