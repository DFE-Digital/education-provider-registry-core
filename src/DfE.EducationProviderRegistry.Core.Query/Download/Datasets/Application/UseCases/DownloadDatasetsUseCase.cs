using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases;

public sealed class DownloadDatasetsUseCase :
    IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>
{
    private readonly ILogger<DownloadDatasetsUseCase> _logger;
    private readonly IDatasetDownloadRepository _datasetDownloadRepository;

    public DownloadDatasetsUseCase(
        IDatasetDownloadRepository datasetDownloadRepository,
        ILogger<DownloadDatasetsUseCase> logger)
    {
        _datasetDownloadRepository = datasetDownloadRepository ??
            throw new ArgumentNullException(nameof(datasetDownloadRepository));
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UseCaseResponse<DownloadDatasetsResponse>> HandleRequestAsync(
        DownloadDatasetsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Dataset? model =
                await _datasetDownloadRepository
                    .GetDatasetByName(request.Filename, cancellationToken);

            DownloadDatasetsResponse response = new(model);

            return UseCaseResponse<DownloadDatasetsResponse>.Success(response);
        }
        catch (OperationCanceledException ex)
        {
            const string message = "The download request was cancelled by the caller.";

            _logger.LogWarning(
                ex,
                "{UseCase} execution cancelled: {Message}",
                nameof(DownloadDatasetsUseCase),
                message);

            return UseCaseResponse<DownloadDatasetsResponse>.Failure(message);
        }
        catch (DownloadException ex)
        {
            const string message = "A domain-specific error occurred during download.";

            _logger.LogError(
                ex,
                "{UseCase} domain-specific error: {Message}",
                nameof(DownloadDatasetsUseCase),
                message);

            return UseCaseResponse<DownloadDatasetsResponse>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message = "An unexpected error occurred while processing the download request.";

            _logger.LogError(
                ex,
                "{UseCase} unexpected error: {Message}",
                nameof(DownloadDatasetsUseCase),
                message);

            return UseCaseResponse<DownloadDatasetsResponse>.Failure(message);
        }
    }
}
