using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.Exporters.Csv.TestDoubles;

public sealed class FakePostgresCsvReader : IDownloadExporter
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

    public IQueryable<object>? Query { get; private set; }

    public int CopyCount { get; private set; }

    public CancellationToken CancellationToken
    {
        get;
        private set;
    }

    public async Task ExportToStreamAsync(
        Stream targetStream,
        IQueryable<object> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(targetStream);
        ArgumentNullException.ThrowIfNull(query);

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
