using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvExporterTests
{
    [Fact]
    public void Constructor_NullReader_ThrowsArgumentNullException()
    {
        // Act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new PostgresCsvExporter(null!));

        // Assert
        Assert.Equal(
            "reader",
            exception.ParamName);
    }

    [Fact]
    public async Task ExportToStreamAsync_NullTargetStream_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IPostgresCsvReader> postgresCsvReader =
            new(MockBehavior.Strict);

        PostgresCsvExporter exporter =
            new(postgresCsvReader.Object);

        // Act
        ArgumentNullException exception =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                exporter.ExportToStreamAsync(
                    null!,
                    "SELECT id FROM educational_establishments",
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(
            "targetStream",
            exception.ParamName);

        postgresCsvReader.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExportToStreamAsync_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IPostgresCsvReader> postgresCsvReader =
            new(MockBehavior.Strict);

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
            new(MockBehavior.Strict);

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
    public async Task ExportToStreamAsync_ValidArguments_DelegatesToReader()
    {
        // Arrange
        const string query =
            "SELECT id, name FROM educational_establishments ORDER BY id";

        CancellationToken cancellationToken =
            TestContext.Current.CancellationToken;

        Mock<IPostgresCsvReader> postgresCsvReader =
            new(MockBehavior.Strict);

        await using MemoryStream targetStream =
            new();

        postgresCsvReader
            .Setup(reader => reader.CopyToStreamAsync(
                targetStream,
                query,
                cancellationToken))
            .Returns(Task.CompletedTask);

        PostgresCsvExporter exporter =
            new(postgresCsvReader.Object);

        // Act
        await exporter.ExportToStreamAsync(
            targetStream,
            query,
            cancellationToken);

        // Assert
        postgresCsvReader.Verify(
            reader => reader.CopyToStreamAsync(
                targetStream,
                query,
                cancellationToken),
            Times.Once);

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
        string csvOutput =
            Encoding.UTF8.GetString(
                targetStream.ToArray());

        Assert.Equal(csv, csvOutput);
        Assert.Equal(query, postgresCsvReader.Query);
        Assert.Equal(1, postgresCsvReader.CopyCount);
    }

    [Fact]
    public async Task ExportToStreamAsync_ValidQuery_LeavesTargetStreamOpen()
    {
        // Arrange
        FakePostgresCsvReader postgresCsvReader =
            PostgresCsvReaderTestDouble.CreateFake(
                "id,name\n1,Academy One\n");

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
        using CancellationTokenSource cancellationTokenSource =
            new();

        CancellationToken cancellationToken =
            cancellationTokenSource.Token;

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
            postgresCsvReader.CancellationToken);
    }

    [Fact]
    public async Task ExportToStreamAsync_ReaderFails_PropagatesException()
    {
        // Arrange
        const string query =
            "SELECT id FROM educational_establishments";

        CancellationToken cancellationToken =
            TestContext.Current.CancellationToken;

        IOException expectedException =
            new("Reading failed.");

        Mock<IPostgresCsvReader> postgresCsvReader =
            new(MockBehavior.Strict);

        await using MemoryStream targetStream =
            new();

        postgresCsvReader
            .Setup(reader => reader.CopyToStreamAsync(
                targetStream,
                query,
                cancellationToken))
            .ThrowsAsync(expectedException);

        PostgresCsvExporter exporter =
            new(postgresCsvReader.Object);

        // Act
        IOException actualException =
            await Assert.ThrowsAsync<IOException>(() =>
                exporter.ExportToStreamAsync(
                    targetStream,
                    query,
                    cancellationToken));

        // Assert
        Assert.Same(
            expectedException,
            actualException);

        postgresCsvReader.Verify(
            reader => reader.CopyToStreamAsync(
                targetStream,
                query,
                cancellationToken),
            Times.Once);

        postgresCsvReader.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExportToStreamAsync_CalledTwice_UsesReaderForBothExports()
    {
        // Arrange
        const string csv =
            """
            id,name
            1,Academy One

            """;

        FakePostgresCsvReader postgresCsvReader =
            PostgresCsvReaderTestDouble.CreateFake(csv);

        PostgresCsvExporter exporter =
            new(postgresCsvReader);

        await using MemoryStream firstTargetStream =
            new();

        await using MemoryStream secondTargetStream =
            new();

        // Act
        await exporter.ExportToStreamAsync(
            firstTargetStream,
            "SELECT id, name FROM first_table",
            TestContext.Current.CancellationToken);

        await exporter.ExportToStreamAsync(
            secondTargetStream,
            "SELECT id, name FROM second_table",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            csv,
            Encoding.UTF8.GetString(
                firstTargetStream.ToArray()));

        Assert.Equal(
            csv,
            Encoding.UTF8.GetString(
                secondTargetStream.ToArray()));

        Assert.Equal(
            2,
            postgresCsvReader.CopyCount);

        Assert.Equal(
            "SELECT id, name FROM second_table",
            postgresCsvReader.Query);
    }
}
