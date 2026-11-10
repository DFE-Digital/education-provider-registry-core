using System.Buffers;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvExporter : IPostgresCsvExporter
{
    private const int BufferSize = 32 * 1024;
    private readonly IPostgresCsvReader _postgresCsvReader;
    private readonly ArrayPool<char> _bufferPool;

    public PostgresCsvExporter(
        IPostgresCsvReader postgresCsvReader)
        : this(
            postgresCsvReader,
            ArrayPool<char>.Shared){
    }

    internal PostgresCsvExporter(
        IPostgresCsvReader postgresCsvReader,
        ArrayPool<char> bufferPool)
    {
        ArgumentNullException.ThrowIfNull(postgresCsvReader);
        ArgumentNullException.ThrowIfNull(bufferPool);

        _postgresCsvReader = postgresCsvReader;
        _bufferPool = bufferPool;
    }

    public async Task ExportToStreamAsync(
        Stream targetStream,
        string query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(targetStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        await _postgresCsvReader.OpenAsync(
            query,
            cancellationToken);

        await using StreamWriter writer = new(
            targetStream,
            leaveOpen: true);

        char[] buffer =
            _bufferPool.Rent(BufferSize);

        try
        {
            while (true)
            {
                int charsRead =
                    await _postgresCsvReader.ReadAsync(
                        buffer.AsMemory(0, BufferSize),
                        cancellationToken);

                if (charsRead == 0)
                {
                    break;
                }

                await writer.WriteAsync(
                    buffer.AsMemory(0, charsRead),
                    cancellationToken);
            }

            await writer.FlushAsync(
                cancellationToken);
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }
}
