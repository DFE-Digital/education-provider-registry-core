using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvReader : IPostgresCsvReader
{
    private const string ConnectionStringKey = "eprweb_eprdat_dotnet_db_connection";
    private const string CopyQueryTemplate = "COPY ({0}) TO STDOUT WITH CSV HEADER";

    private readonly IConfiguration _configuration;

    private NpgsqlConnection? _connection;
    private NpgsqlCopyTextReader? _reader;
    private bool _disposed;

    public PostgresCsvReader(
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _configuration = configuration;
    }

    public async Task OpenAsync(
        string query,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);

        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        if (_connection is not null || _reader is not null)
        {
            throw new InvalidOperationException(
                "The PostgreSQL CSV reader has already been opened.");
        }

        string connectionString =
            _configuration[ConnectionStringKey]
            ?? throw new InvalidOperationException(
                $"Configuration value '{ConnectionStringKey}' is missing.");

        string copyQuery = string.Format(
            CopyQueryTemplate,
            query);

        NpgsqlConnection connection =
            new(connectionString);

        try
        {
            await connection.OpenAsync(cancellationToken);

            NpgsqlCopyTextReader reader =
                await connection.BeginTextExportAsync(
                    copyQuery,
                    cancellationToken);

            _connection = connection;
            _reader = reader;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    public ValueTask<int> ReadAsync(
        Memory<char> buffer,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);

        NpgsqlCopyTextReader reader =
            _reader
            ?? throw new InvalidOperationException(
                "The PostgreSQL CSV reader has not been opened.");

        return reader.ReadAsync(
            buffer,
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            _reader?.Dispose();
        }
        finally
        {
            _reader = null;

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        GC.SuppressFinalize(this);
    }
}
