using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv.TestDoubles;

public static class PostgresCsvReaderTestDouble
{
    public static Mock<IPostgresCsvReader> Mock() => new(MockBehavior.Strict);

    public static FakePostgresCsvReader CreateFake(string csv) => new(csv);

    public static Mock<IPostgresCsvReader> MockFor(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        Mock<IPostgresCsvReader> mock =
            new(MockBehavior.Strict);

        mock
            .Setup(reader => reader.CopyToStreamAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        return mock;
    }
}
