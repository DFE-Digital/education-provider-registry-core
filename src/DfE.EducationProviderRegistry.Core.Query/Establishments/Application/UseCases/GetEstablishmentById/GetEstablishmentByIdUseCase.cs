using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;

public class GetEstablishmentByIdUseCase :
    IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsModel?>>
{
    private readonly ILogger<GetEstablishmentByIdUseCase> _logger;
    private readonly IEstablishmentsRepository _establishmentRepository;

    public GetEstablishmentByIdUseCase(
        ILogger<GetEstablishmentByIdUseCase> logger,
        IEstablishmentsRepository establishmentRepository)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(establishmentRepository);

        _logger = logger;
        _establishmentRepository = establishmentRepository;
    }

    public async Task<UseCaseResponse<EstablishmentDetailsModel?>> HandleRequestAsync(
        GetEstablishmentByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            EstablishmentUrnModel establishmentId = EstablishmentUrnModel.Create(request.Urn);
            EstablishmentDetailsModel? establishment = await _establishmentRepository
                .GetEstablishmentById(establishmentId, cancellationToken);

            return UseCaseResponse<EstablishmentDetailsModel?>.Success(establishment);
        }
        catch (OperationCanceledException ex)
        {
            const string message =
                "The request was cancelled by the caller.";

            _logger.LogError(
                ex,
                "{UseCase} execution was cancelled by the caller: {Message}",
                nameof(GetEstablishmentByIdUseCase),
                message
                );

            return UseCaseResponse<EstablishmentDetailsModel?>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message =
                "An unexpected error occurred while processing the request.";

            _logger.LogError(
                ex,
                "{UseCase} encountered an unexpected error: {Message}",
                nameof(GetEstablishmentByIdUseCase),
                message);

            return UseCaseResponse<EstablishmentDetailsModel?>.Failure(message);
        }
    }
}
