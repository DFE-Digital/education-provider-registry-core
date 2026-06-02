using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;

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
    IUseCaseRequest<UseCaseResponse<IReadOnlyCollection<Establishment>>>
{
    /// <summary>
    /// Creates a new <see cref="GetEstablishmentsRequest"/> instance.
    /// This factory method provides a clear, intention‑revealing way
    /// to construct the request.
    /// </summary>
    /// <returns>
    /// A new <see cref="GetEstablishmentsRequest"/> instance.
    /// </returns>
    public static GetEstablishmentsRequest Create() => new();
}
