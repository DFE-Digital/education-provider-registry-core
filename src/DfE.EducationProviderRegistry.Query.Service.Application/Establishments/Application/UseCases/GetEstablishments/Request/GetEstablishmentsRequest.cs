using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;

namespace DfE.GIAS2.Query.Service.Core.Establishments.Application.UseCases.GetEstablishments.Request;

/// <summary>
/// Represents a request to retrieve all establishments from the
/// Education Provider Registry query service.
/// </summary>
/// <remarks>
/// This request type carries no parameters and serves as a simple
/// trigger for the corresponding use case. The response contains a
/// read‑only collection of <see cref="Establishment"/> instances.
/// </remarks>
public sealed record GetEstablishmentsRequest :
    IUseCaseRequest<UseCaseResponse<IReadOnlyCollection<Establishment>>>;
