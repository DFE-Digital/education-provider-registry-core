using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.GIAS2.Query.Service.Core.Establishments.Infrastructure;
using Microsoft.Extensions.Logging;

namespace DfE.GIAS2.Query.Service.Core.Establishments.Application.UseCases.GetEstablishments;

/// <summary>
/// Executes the retrieval of establishment data using the
/// <see cref="IEstablishmentsRepository"/> and returns the results
/// wrapped in a <see cref="UseCaseResponse{T}"/>.
/// </summary>
/// <remarks>
/// This use case accepts a <see cref="GetEstablishmentsRequest"/>,
/// which specifies the required field names to validate before execution.
/// Structured logging is used throughout to record execution flow,
/// cancellation events, and error conditions.
/// </remarks>
public sealed class GetEstablishmentsUseCase :
    IUseCase<
        GetEstablishmentsRequest,
        UseCaseResponse<IReadOnlyCollection<Establishment>>>
{
    private readonly ILogger<GetEstablishmentsUseCase> _logger;
    private readonly IEstablishmentsRepository _establishmentsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEstablishmentsUseCase"/> class.
    /// </summary>
    /// <param name="logger">The logger used for structured diagnostic logging.</param>
    /// <param name="establishmentsRepository">
    /// The repository responsible for retrieving establishment data.
    /// </param>
    public GetEstablishmentsUseCase(
        ILogger<GetEstablishmentsUseCase> logger,
        IEstablishmentsRepository establishmentsRepository)
    {
        _logger = logger;
        _establishmentsRepository = establishmentsRepository;
    }

    /// <summary>
    /// Handles the request to retrieve establishments and returns the results
    /// wrapped in a <see cref="UseCaseResponse{T}"/>.
    /// </summary>
    /// <param name="request">
    /// The request containing the validated required field names for the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="UseCaseResponse{T}"/> containing the retrieved establishments,
    /// or an error response if the operation fails or is cancelled.
    /// </returns>
    public async Task<UseCaseResponse<IReadOnlyCollection<Establishment>>> HandleRequestAsync(
        GetEstablishmentsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IReadOnlyCollection<Establishment> results =
                await _establishmentsRepository.GetEstablishments(cancellationToken);

            return UseCaseResponse<IReadOnlyCollection<Establishment>>.Success(results);
        }
        catch (OperationCanceledException)
        {
            _logger.LogError(
                "{UseCase} execution was cancelled by the caller.",
                nameof(GetEstablishmentsUseCase));

            return UseCaseResponse<IReadOnlyCollection<Establishment>>
                .Failure("The request was cancelled.");
        }
        catch (EstablishmentException ex)
        {
            const string message =
                "Failed to retrieve establishment information from the repository.";

            _logger.LogError(
                ex,
                "{UseCase} encountered a domain-specific error: {Message}",
                nameof(GetEstablishmentsUseCase),
                message);

            return UseCaseResponse<IReadOnlyCollection<Establishment>>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected error occurred while processing the request.";

            _logger.LogError(
                ex,
                "{UseCase} encountered an unexpected error: {Message}",
                nameof(GetEstablishmentsUseCase),
                message);

            return UseCaseResponse<IReadOnlyCollection<Establishment>>.Failure(message);
        }
    }
}
