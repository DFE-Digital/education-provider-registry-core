using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Application.UseCases;

public sealed class DownloadUseCase : IUseCase<DownloadRequest, UseCaseResponse<DownloadResponse>>
{
    private readonly ILogger<SearchUseCase> _logger;

    public DownloadUseCase(
        ILogger<SearchUseCase> logger)
    {
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UseCaseResponse<DownloadResponse>> HandleRequestAsync(
        DownloadRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            DownloadResponse model = new();

            return UseCaseResponse<DownloadResponse>.Success(model);
        }
        catch (OperationCanceledException ex)
        {
            const string message = "The download request was cancelled by the caller.";

            _logger.LogWarning(
                ex,
                "{UseCase} execution cancelled: {Message}",
                nameof(DownloadUseCase),
                message);

            return UseCaseResponse<DownloadResponse>.Failure(message);
        }
        catch (SearchException ex)
        {
            const string message = "A domain-specific error occurred during download.";

            _logger.LogError(
                ex,
                "{UseCase} domain-specific error: {Message}",
                nameof(SearchUseCase),
                message);

            return UseCaseResponse<DownloadResponse>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message = "An unexpected error occurred while processing the download request.";

            _logger.LogError(
                ex,
                "{UseCase} unexpected error: {Message}",
                nameof(SearchUseCase),
                message);

            return UseCaseResponse<DownloadResponse>.Failure(message);
        }
    }
}
