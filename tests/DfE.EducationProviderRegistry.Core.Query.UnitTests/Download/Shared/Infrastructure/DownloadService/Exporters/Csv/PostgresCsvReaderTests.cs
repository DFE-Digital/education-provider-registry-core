using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvReaderUnitTests
{
    private const string ConnectionStringKey = "eprweb_eprdat_dotnet_db_connection";

    [Fact]
    public void Constructor_NullConfiguration_ThrowsArgumentNullException()
    {
        // Act/Assert
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
            new PostgresCsvReader(null!));

        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public async Task OpenAsync_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        PostgresCsvReader reader = new (configuration);
        await reader.DisposeAsync();

        // Act/Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            reader.OpenAsync("SELECT * FROM educational_establishments", TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task OpenAsync_NullOrWhitespaceQuery_ThrowsArgumentException(string? invalidQuery)
    {
        // Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        await using PostgresCsvReader reader = new(configuration);

        // Act/Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            reader.OpenAsync(invalidQuery!, TestContext.Current.CancellationToken));

        Assert.Equal("query", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    public async Task OpenAsync_NullOrWhitespaceQuery_ThrowsArgumentNuillException(string? invalidQuery)
    {
        // Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        await using PostgresCsvReader reader = new(configuration);

        // Act/Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            reader.OpenAsync(invalidQuery!, TestContext.Current.CancellationToken));

        Assert.Equal("query", exception.ParamName);
    }

    [Fact]
    public async Task OpenAsync_MissingConfiguration_ThrowsInvalidOperationException()
    {
        // Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        await using PostgresCsvReader reader = new(configuration);

        // Act/Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            reader.OpenAsync("SELECT * FROM educational_establishments", TestContext.Current.CancellationToken));

        Assert.Contains(ConnectionStringKey, exception.Message);
    }

    [Fact]
    public async Task ReadAsync_WhenDisposed_ThrowsObjectDisposedException()
    {
        // Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        PostgresCsvReader reader = new(configuration);
        await reader.DisposeAsync();
        Memory<char> buffer = new char[1024];

        // Act/Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            reader.ReadAsync(buffer, TestContext.Current.CancellationToken).AsTask());
    }

    [Fact]
    public async Task ReadAsync_WhenNotOpened_ThrowsInvalidOperationException()
    {
        // Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        await using PostgresCsvReader reader = new(configuration);
        Memory<char> buffer = new char[1024];

        // Act/Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            reader.ReadAsync(buffer, TestContext.Current.CancellationToken).AsTask());

        Assert.Equal("The PostgreSQL CSV reader has not been opened.", exception.Message);
    }

    [Fact]
    public async Task DisposeAsync_MultipleCalls_IsIdempotentAndSafe()
    {
        // Arrange
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        PostgresCsvReader reader = new(configuration);

        // Act/Assert
        await reader.DisposeAsync();
        await reader.DisposeAsync();
    }
}
