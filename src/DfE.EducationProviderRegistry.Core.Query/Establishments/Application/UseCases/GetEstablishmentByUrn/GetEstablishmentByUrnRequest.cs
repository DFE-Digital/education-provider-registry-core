using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentByUrn;

public sealed record GetEstablishmentByUrnRequest(string Urn) : IUseCaseRequest<UseCaseResponse<EstablishmentDetailsReadModel>>;
