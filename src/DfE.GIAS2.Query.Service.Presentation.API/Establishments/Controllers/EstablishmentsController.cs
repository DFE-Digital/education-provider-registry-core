using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.UseCases.GetEstablishments.Request;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DfE.GIAS2.Query.Service.Presentation.API.Establishments.Controllers;

/// <summary>
/// Provides API endpoints for retrieving establishment information from the
/// GIAS2 query service. This controller exposes health checks and data‑retrieval
/// operations for client applications.
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class EstablishmentsController : ControllerBase
{
    private readonly ILogger<EstablishmentsController> _logger;
    private readonly IUseCase<
        GetEstablishmentsRequest,
        UseCaseResponse<IReadOnlyCollection<Establishment>>> _getEstablishmentsUseCase;
    private readonly IMapper<Establishment, object?> _modelToViewModelMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentsController"/> class.
    /// </summary>
    /// <param name="logger">The logger used for diagnostic and operational logging.</param>
    /// <param name="getEstablishmentsUseCase">
    /// The use case responsible for retrieving establishment data.
    /// </param>
    /// <param name="modelToViewModelMapper">
    /// The mapper used to convert domain <see cref="Establishment"/> models into
    /// presentation‑ready view models.
    /// </param>
    public EstablishmentsController(
        ILogger<EstablishmentsController> logger,
        IUseCase<
            GetEstablishmentsRequest,
            UseCaseResponse<IReadOnlyCollection<Establishment>>> getEstablishmentsUseCase,
        IMapper<Establishment, object?> modelToViewModelMapper)
    {
        _logger = logger;
        _getEstablishmentsUseCase = getEstablishmentsUseCase;
        _modelToViewModelMapper = modelToViewModelMapper;
    }

    /// <summary>
    /// Performs a lightweight health check to confirm that the API is running.
    /// </summary>
    /// <returns>
    /// A 200 OK response containing a simple status message and timestamp.
    /// </returns>
    [HttpGet("health", Name = "HealthCheck")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Service is running",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Retrieves all establishments from the GIAS2 query service.
    /// </summary>
    /// <param name="requiredEstablishmentFields">
    /// A placeholder for future query parameters. Currently unused.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that allows the request to be cancelled by the client.
    /// </param>
    /// <returns>
    /// A streamed 200 OK response containing establishment view models,
    /// or an appropriate error response if the use case fails.
    /// </returns>
    [HttpGet(Name = "GetEstablishments")]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken = default)
    {
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await _getEstablishmentsUseCase
                .HandleRequestAsync(
                    GetEstablishmentsRequest.Create(),
                    cancellationToken);

        if (!result.SuccessfulRequest)
        {
            return Problem(
                detail: result.ErrorMessage ?? "Unknown error",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (!result.HasModel())
        {
            return Problem(
                detail: "Use case returned no data.",
                statusCode: StatusCodes.Status404NotFound);
        }

#pragma warning disable CS1998
        /// <summary>
        /// Streams establishment view models to the client as an asynchronous sequence.
        /// </summary>
        /// <param name="ct">A cancellation token for the streaming operation.</param>
        /// <returns>
        /// An asynchronous sequence of mapped establishment view models.
        /// </returns>
        async IAsyncEnumerable<object?> StreamResults(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (Establishment establishment in result.Model!)
            {
                ct.ThrowIfCancellationRequested();
                yield return _modelToViewModelMapper.Map(establishment);
            }
        }
#pragma warning restore CS1998

        return Ok(StreamResults(cancellationToken));
    }
}
