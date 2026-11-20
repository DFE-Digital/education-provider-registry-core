using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;

public sealed class PostgresCsvDownloadExporter : IDownloadExporter
{
    private const string ConnectionStringKey = "eprweb_eprdat_dotnet_db_connection";
    private const string CopyQueryTemplate = "COPY ({0}) TO STDOUT WITH CSV HEADER";
    private const int BufferSize = 81920;

    private readonly IConfiguration _configuration;

    public PostgresCsvDownloadExporter(
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _configuration = configuration;
    }

    public async Task ExportToStreamAsync(
        Stream targetStream,
        IQueryable<object> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(targetStream);
        ArgumentNullException.ThrowIfNull(query);

        string connectionString =
            _configuration[ConnectionStringKey]
            ?? throw new InvalidOperationException(
                $"Configuration value '{ConnectionStringKey}' is missing.");

        string copyQuery =
            string.Format(CopyQueryTemplate, query.ToQueryString());

        await using NpgsqlConnection connection =
            new(connectionString);

        await connection.OpenAsync(cancellationToken);

        await using NpgsqlCopyTextReader reader =
            await connection.BeginTextExportAsync(
                copyQuery,
                cancellationToken);

        using StreamWriter writer =
            new(targetStream, leaveOpen: true);

        char[] buffer = new char[BufferSize];

        int charsRead;

        while ((charsRead = await reader.ReadAsync(
                   buffer,
                   cancellationToken)) > 0)
        {
            await writer.WriteAsync(
                buffer.AsMemory(0, charsRead),
                cancellationToken);
        }

        await writer.FlushAsync(cancellationToken);
    }
}
