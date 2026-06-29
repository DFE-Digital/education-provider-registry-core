using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;

public sealed record GetEstablishmentByIdRequest(string Urn) :
    IUseCaseRequest<UseCaseResponse<EstablishmentDetailsModel?>>;
