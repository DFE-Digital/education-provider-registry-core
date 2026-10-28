using System.Buffers;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.CsvExporter;

public sealed class PostgresCsvExporter : IPostgresCsvExporter
{
    private const string ConnectionStringKey = "eprweb_eprdat_dotnet_db_connection";
    private const string CopyQueryTemplate = "COPY ({0}) TO STDOUT WITH CSV HEADER";
    private const int BufferSize = 32 * 1024;

    private readonly IConfiguration _configuration;

    public PostgresCsvExporter(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ExportAsync(
        string outputPath,
        string query,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        string connectionString =
            _configuration[ConnectionStringKey]
            ?? throw new InvalidOperationException(
                $"Configuration value '{ConnectionStringKey}' is missing.");

        await using NpgsqlConnection connection =
            new(connectionString);

        await connection.OpenAsync(cancellationToken);

        await using FileStream fileStream =
            new(
                outputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                useAsync: true);

        await using StreamWriter writer =
            new(fileStream);

        string copyQuery =
            string.Format(
                CopyQueryTemplate,
                query);

        using NpgsqlCopyTextReader reader =
            await connection.BeginTextExportAsync(
                copyQuery,
                cancellationToken);

        char[] buffer =
            ArrayPool<char>.Shared.Rent(BufferSize);

        try
        {
            while (true)
            {
                int charsRead =
                    await reader.ReadAsync(
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

            await writer.FlushAsync(cancellationToken);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }
}
