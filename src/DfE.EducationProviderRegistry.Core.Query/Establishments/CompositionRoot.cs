using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments;

public static class CompositionRoot
{
    public static IServiceCollection AddEstablishmentsUseCaseDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<
                IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>>,
                GetEstablishmentsUseCase>()

            .AddScoped<
                IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsModel?>>,
                GetEstablishmentByIdUseCase>();
    }

    public static IServiceCollection AddEstablishmentsInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<IEstablishmentsRepository, EfPostgresEstablishmentRepository>()

            .AddSingleton<IMapper<
                IEnumerable<Establishment>,
                IReadOnlyCollection<EstablishmentDetailsModel>>,
                    EstablishmentsToDetailsModelMapper>()

            .AddSingleton<IMapper<
                Establishment, EstablishmentDetailsModel>,
                    EstablishmentToDetailsModelMapper>();
    }
}
