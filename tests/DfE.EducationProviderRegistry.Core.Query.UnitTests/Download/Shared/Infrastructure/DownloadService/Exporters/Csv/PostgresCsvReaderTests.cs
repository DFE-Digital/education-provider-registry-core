using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvReaderUnitTests
{
    private const string ConnectionStringKey =
        "eprweb_eprdat_dotnet_db_connection";

    [Fact]
    public void Constructor_NullConfiguration_ThrowsArgumentNullException()
    {
        // Act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new PostgresCsvReader(null!));

        // Assert
        Assert.Equal(
            "configuration",
            exception.ParamName);
    }

    [Fact]
    public async Task CopyToStreamAsync_NullTargetStream_ThrowsArgumentNullException()
    {
        // Arrange
        IConfigurationRoot configuration =
            new ConfigurationBuilder()
                .Build();

        PostgresCsvReader reader =
            new(configuration);

        // Act
        ArgumentNullException exception =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                reader.CopyToStreamAsync(
                    null!,
                    "SELECT * FROM educational_establishments",
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(
            "targetStream",
            exception.ParamName);
    }

    [Fact]
    public async Task CopyToStreamAsync_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        IConfigurationRoot configuration =
            new ConfigurationBuilder()
                .Build();

        PostgresCsvReader reader =
            new(configuration);

        await using MemoryStream targetStream =
            new();

        // Act
        ArgumentNullException exception =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                reader.CopyToStreamAsync(
                    targetStream,
                    null!,
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(
            "query",
            exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    public async Task CopyToStreamAsync_EmptyOrWhitespaceQuery_ThrowsArgumentException(
        string invalidQuery)
    {
        // Arrange
        IConfigurationRoot configuration =
            new ConfigurationBuilder()
                .Build();

        PostgresCsvReader reader =
            new(configuration);

        await using MemoryStream targetStream =
            new();

        // Act
        ArgumentException exception =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                reader.CopyToStreamAsync(
                    targetStream,
                    invalidQuery,
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(
            "query",
            exception.ParamName);
    }

    [Fact]
    public async Task CopyToStreamAsync_MissingConfiguration_ThrowsInvalidOperationException()
    {
        // Arrange
        IConfigurationRoot configuration =
            new ConfigurationBuilder()
                .Build();

        PostgresCsvReader reader =
            new(configuration);

        await using MemoryStream targetStream =
            new();

        // Act
        InvalidOperationException exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                reader.CopyToStreamAsync(
                    targetStream,
                    "SELECT * FROM educational_establishments",
                    TestContext.Current.CancellationToken));

        // Assert
        Assert.Contains(
            ConnectionStringKey,
            exception.Message);
    }
}
