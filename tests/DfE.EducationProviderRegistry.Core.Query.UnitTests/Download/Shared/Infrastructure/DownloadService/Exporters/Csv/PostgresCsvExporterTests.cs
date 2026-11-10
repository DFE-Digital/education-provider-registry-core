using System.Buffers;
using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvExporterTests
{
    private const int BufferSize = 32 * 1024;

    [Fact]
    public async Task ExportToStreamAsync_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IPostgresCsvReader> postgresCsvReader =
            PostgresCsvReaderTestDouble.Mock();

        PostgresCsvExporter exporter =
            new(postgresCsvReader.Object);

        await using MemoryStream targetStream =
            new();

        // Act
        ArgumentNullException exception =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                exporter.ExportToStreamAsync(
                    targetStream,
                    null!,
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(
            "query",
            exception.ParamName);

        postgresCsvReader.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    public async Task ExportToStreamAsync_EmptyOrWhiteSpaceQuery_ThrowsArgumentException(
        string invalidQuery)
    {
        // Arrange
        Mock<IPostgresCsvReader> postgresCsvReader =
            PostgresCsvReaderTestDouble.Mock();

        PostgresCsvExporter exporter =
            new(postgresCsvReader.Object);

        await using MemoryStream targetStream =
            new();

        // Act
        ArgumentException exception =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                exporter.ExportToStreamAsync(
                    targetStream,
                    invalidQuery,
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(
            "query",
            exception.ParamName);

        postgresCsvReader.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExportToStreamAsync_ValidQuery_StreamsCsvSuccessfully()
    {
        // Arrange
        const string query =
            "SELECT id, name FROM educational_establishments ORDER BY id";

        const string csv =
            """
            id,name
            1,Academy One
            2,School Two

            """;

        FakePostgresCsvReader postgresCsvReader =
            PostgresCsvReaderTestDouble.CreateFake(csv);

        PostgresCsvExporter exporter =
            new(postgresCsvReader);

        await using MemoryStream targetStream =
            new();

        // Act
        await exporter.ExportToStreamAsync(
            targetStream,
            query,
            TestContext.Current.CancellationToken);

        // Assert
        targetStream.Position = 0;

        using StreamReader reader =
            new(
                targetStream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: true);

        string csvOutput =
            await reader.ReadToEndAsync(
                TestContext.Current.CancellationToken);

        Assert.Equal(csv, csvOutput);
        Assert.Equal(query, postgresCsvReader.Query);
        Assert.Equal(1, postgresCsvReader.OpenCount);
        Assert.True(postgresCsvReader.ReadCount > 0);
    }

    [Fact]
    public async Task ExportToStreamAsync_MultipleReads_StreamsCompleteCsvSuccessfully()
    {
        // Arrange
        const string csv =
            """
            id,name
            1,Academy One
            2,School Two

            """;

        FakePostgresCsvReader postgresCsvReader =
            PostgresCsvReaderTestDouble.CreateFake(
                csv,
                maximumCharactersPerRead: 5);

        PostgresCsvExporter exporter =
            new(postgresCsvReader);

        await using MemoryStream targetStream =
            new();

        // Act
        await exporter.ExportToStreamAsync(
            targetStream,
            "SELECT id, name FROM educational_establishments",
            TestContext.Current.CancellationToken);

        // Assert
        targetStream.Position = 0;

        using StreamReader reader =
            new(
                targetStream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: true);

        string csvOutput =
            await reader.ReadToEndAsync(
                TestContext.Current.CancellationToken);

        Assert.Equal(csv, csvOutput);
        Assert.True(postgresCsvReader.ReadCount > 1);
    }

    [Fact]
    public async Task ExportToStreamAsync_ValidQuery_LeavesTargetStreamOpen()
    {
        // Arrange
        FakePostgresCsvReader postgresCsvReader =
            PostgresCsvReaderTestDouble.CreateFake("id,name\n1,Academy One\n");

        PostgresCsvExporter exporter =
            new(postgresCsvReader);

        await using MemoryStream targetStream =
            new();

        // Act
        await exporter.ExportToStreamAsync(
            targetStream,
            "SELECT id, name FROM educational_establishments",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.True(targetStream.CanRead);
        Assert.True(targetStream.CanWrite);
    }

    [Fact]
    public async Task ExportToStreamAsync_ValidQuery_PassesCancellationTokenToReader()
    {
        // Arrange
        CancellationToken cancellationToken =
            TestContext.Current.CancellationToken;

        FakePostgresCsvReader postgresCsvReader =
            PostgresCsvReaderTestDouble.CreateFake("id\n1\n");

        PostgresCsvExporter exporter =
            new(postgresCsvReader);

        await using MemoryStream targetStream =
            new();

        // Act
        await exporter.ExportToStreamAsync(
            targetStream,
            "SELECT id FROM educational_establishments",
            cancellationToken);

        // Assert
        Assert.Equal(
            cancellationToken,
            postgresCsvReader.OpenCancellationToken);

        Assert.All(
            postgresCsvReader.ReadCancellationTokens,
            token => Assert.Equal(
                cancellationToken,
                token));
    }

    [Fact]
    public async Task ExportToStreamAsync_ReadFails_ReturnsBuffer()
    {
        // Arrange
        char[] buffer =
            new char[BufferSize];

        Mock<ArrayPool<char>> bufferPool =
            new();

        bufferPool
            .Setup(pool => pool.Rent(BufferSize))
            .Returns(buffer);

        Mock<IPostgresCsvReader> postgresCsvReader =
            PostgresCsvReaderTestDouble.MockFor(
                ValueTask.FromException<int>(
                    new IOException("Reading failed.")));

        PostgresCsvExporter exporter =
            new(
                postgresCsvReader.Object,
                bufferPool.Object);

        await using MemoryStream targetStream =
            new();

        // Act
        IOException exception =
            await Assert.ThrowsAsync<IOException>(() =>
                exporter.ExportToStreamAsync(
                    targetStream,
                    "SELECT id FROM educational_establishments",
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(
            "Reading failed.",
            exception.Message);

        bufferPool.Verify(
            pool => pool.Return(buffer, false),
            Times.Once);
    }

    [Fact]
    public async Task ExportToStreamAsync_ExportCompletes_ReturnsBuffer()
    {
        // Arrange
        char[] buffer =
            new char[BufferSize];

        Mock<ArrayPool<char>> bufferPool =
            new();

        bufferPool
            .Setup(pool => pool.Rent(BufferSize))
            .Returns(buffer);

        Mock<IPostgresCsvReader> postgresCsvReader =
            PostgresCsvReaderTestDouble.MockFor(ValueTask.FromResult(0));

        PostgresCsvExporter exporter =
            new(
                postgresCsvReader.Object,
                bufferPool.Object);

        await using MemoryStream targetStream =
            new();

        // Act
        await exporter.ExportToStreamAsync(
            targetStream,
            "SELECT id FROM educational_establishments",
            TestContext.Current.CancellationToken);

        // Assert
        bufferPool.Verify(
            pool => pool.Rent(BufferSize),
            Times.Once);

        bufferPool.Verify(
            pool => pool.Return(buffer, false),
            Times.Once);
    }
}
