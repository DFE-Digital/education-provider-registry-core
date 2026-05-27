using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Query.Service.Persistence.Establishments;
using DfE.EducationProviderRegistry.Query.Service.Persistence.Establishments.DataTransferObjects;
using DfE.EducationProviderRegistry.Query.Service.Persistence.Establishments.Mappers;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;
using DfE.GIAS2.Query.Service.Core.Establishments.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GIAS2.Query.Service.Persistence.Establishments;

/// <summary>
/// Provides dependency‑registration extensions for the Establishments
/// persistence layer. This composition root wires together repository
/// implementations and mapping components required by the application
/// and presentation layers.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers Establishments‑related infrastructure dependencies with the
    /// application's dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The service collection to which the dependencies will be added.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> instance, enabling
    /// fluent configuration.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddEstablishmentsInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            // Establishment repository.
            .AddScoped<IEstablishmentsRepository, EstablishmentsRepository>()

            // Collection mapper: DTO → application read model.
            .AddSingleton<IMapper<
                IEnumerable<EstablishmentDataTransferObject>,
                IReadOnlyCollection<Establishment>>,
                    EstablishmentsDtoToModelMapper>()

            // Single‑item mapper: DTO → application read model model.
            .AddSingleton<IMapper<
                EstablishmentDataTransferObject, Establishment>,
                    EstablishmentDtoToModelMapper>();
    }
}