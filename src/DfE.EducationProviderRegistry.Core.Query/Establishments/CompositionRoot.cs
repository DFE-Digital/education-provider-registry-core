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

/// <summary>
/// Provides extension methods for registering all Establishment-related
/// application services, validators, and use cases into an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers all Establishment use case dependencies, validation services,
    /// and supporting components required by the Establishments domain.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> into which the dependencies will be registered.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// This method configures all services involved in Establishment-related operations.
    /// It registers the <see cref="GetEstablishmentsUseCase"/> as the implementation of
    /// <see cref="IUseCaseResponseOnly{TUseCaseResponse}"/> for retrieving collections of
    /// <see cref="EstablishmentDetailsModel"/> instances. In addition to the use case, it also registers
    /// the shared <see cref="IRegexValidationService"/>, which provides reusable regular
    /// expression–based validation logic used throughout the Establishments domain.
    ///
    /// The method also registers the domain-specific validators responsible for validating
    /// Establishment address information and Establishment contact details. These validators
    /// are added as singletons because they are stateless and safe to reuse across requests,
    /// whereas the use case is registered as scoped to align with typical request‑based
    /// application lifetimes.
    ///
    /// Together, these registrations ensure that all Establishment-related functionality is
    /// fully wired up and ready for use by the application.
    /// </remarks>
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
            .AddScoped<IEstablishmentsRepository, FakeDataEstablishmentsRepository>()

            // Collection mapper: DTO → application read model.
            .AddSingleton<IMapper<
                IEnumerable<Establishment>,
                IReadOnlyCollection<EstablishmentDetailsModel>>,
                    EstablishmentsToDetailsModelMapper>()

            // Single‑item mapper: DTO → application read model model.
            .AddSingleton<IMapper<
                Establishment, EstablishmentDetailsModel>,
                    EstablishmentToDetailsModelMapper>();
    }
}
