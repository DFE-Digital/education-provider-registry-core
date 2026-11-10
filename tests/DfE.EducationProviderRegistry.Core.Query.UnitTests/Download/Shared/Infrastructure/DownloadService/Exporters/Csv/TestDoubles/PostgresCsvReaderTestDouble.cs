using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv.TestDoubles;

public static class PostgresCsvReaderTestDouble
{
    public static Mock<IPostgresCsvReader> Mock() => new();

    public static Mock<IPostgresCsvReader> MockFor(ValueTask<int> response)
    {
        Mock<IPostgresCsvReader> postgresCsvReader = Mock();
        postgresCsvReader
            .Setup(reader => reader.OpenAsync(
                It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        _ = postgresCsvReader
            .Setup(reader => reader.ReadAsync(
                It.IsAny<Memory<char>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(response);

        return postgresCsvReader;
    }

    public static FakePostgresCsvReader CreateFake(
        string csv,
        int maximumCharactersPerRead = int.MaxValue) => new(csv, maximumCharactersPerRead);
}

public sealed class FakePostgresCsvReader : IPostgresCsvReader
{
    private readonly string _csv;
    private readonly int _maximumCharactersPerRead;

    private int _position;

    public FakePostgresCsvReader(
        string csv,
        int maximumCharactersPerRead = int.MaxValue)
    {
        _csv = csv;
        _maximumCharactersPerRead =
            maximumCharactersPerRead;
    }

    public string? Query { get; private set; }

    public int OpenCount { get; private set; }

    public int ReadCount { get; private set; }

    public CancellationToken OpenCancellationToken
    {
        get;
        private set;
    }

    public List<CancellationToken> ReadCancellationTokens
    {
        get;
    } = new();

    public Task OpenAsync(
        string query,
        CancellationToken cancellationToken)
    {
        Query = query;
        OpenCount++;
        OpenCancellationToken =
            cancellationToken;

        return Task.CompletedTask;
    }

    public ValueTask<int> ReadAsync(
        Memory<char> buffer,
        CancellationToken cancellationToken)
    {
        ReadCount++;

        ReadCancellationTokens.Add(
            cancellationToken);

        if (_position >= _csv.Length)
        {
            return ValueTask.FromResult(0);
        }

        int remaining =
            _csv.Length - _position;

        int charactersToCopy =
            Math.Min(
                remaining,
                Math.Min(
                    buffer.Length,
                    _maximumCharactersPerRead));

        _csv
            .AsMemory(
                _position,
                charactersToCopy)
            .CopyTo(buffer);

        _position += charactersToCopy;

        return ValueTask.FromResult(
            charactersToCopy);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
