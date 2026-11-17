using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv.TestDoubles;

public sealed class FakePostgresCsvReader : IPostgresCsvReader
{
    private readonly string _csv;
    private readonly int _maximumCharactersPerWrite;

    public FakePostgresCsvReader(
        string csv,
        int maximumCharactersPerWrite = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(csv);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumCharactersPerWrite);

        _csv = csv;
        _maximumCharactersPerWrite =
            maximumCharactersPerWrite;
    }

    public string? Query { get; private set; }

    public int CopyCount { get; private set; }

    public CancellationToken CancellationToken
    {
        get;
        private set;
    }

    public async Task CopyToStreamAsync(
        Stream targetStream,
        string query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(targetStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        Query = query;
        CopyCount++;
        CancellationToken = cancellationToken;

        int position = 0;

        while (position < _csv.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int remaining =
                _csv.Length - position;

            int charactersToWrite =
                Math.Min(
                    remaining,
                    _maximumCharactersPerWrite);

            byte[] bytes =
                Encoding.UTF8.GetBytes(
                    _csv.AsSpan(
                        position,
                        charactersToWrite).ToArray());

            await targetStream.WriteAsync(
                bytes,
                cancellationToken);

            position += charactersToWrite;
        }
    }
}
