using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search;

/// <summary>
/// Provides extension methods for registering all Search-related application
/// services, configuration bindings, and use case implementations into an
/// <see cref="IServiceCollection"/>. This acts as the composition root for the
/// Search feature, ensuring that all required dependencies are correctly wired
/// for runtime execution.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers all Search feature dependencies, including service adapters,
    /// use cases, and strongly typed configuration objects, into the provided
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">
    /// The service collection into which Search-related dependencies will be registered.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="services"/> argument is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddSearchDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register the search service adapter and use case.
        services
            .AddScoped<
                ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>,
                DummySearchServiceAdapter>()
            .AddScoped<
                IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>,
                SearchUseCase>();

        // Bind SearchCriteria configuration from appsettings.json.
        services.AddOptions<SearchCriteria>()
            .Configure<IConfiguration>((settings, configuration) =>
                configuration
                    .GetSection(nameof(SearchCriteria))
                    .Bind(settings));

        // Register the resolved SearchCriteria instance for direct injection.
        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<SearchCriteria>>().Value);

        return services;
    }
}
