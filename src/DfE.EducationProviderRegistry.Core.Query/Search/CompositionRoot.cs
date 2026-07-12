using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Formatters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Search;

/// <summary>
/// Provides dependency‑injection registration for all infrastructure‑level
/// search components used by the MVC application. This includes orchestrators,
/// providers, pipeline steps, mappers, filter expression builders, and
/// supporting metadata resolvers.
/// </summary>
/// <remarks>
/// This composition root is responsible for wiring together the complete
/// search pipeline, including trigram search orchestration, facet providers,
/// projection builders, SQL execution, filter expression factories, and
/// logical operator factories. All registrations are scoped or singleton
/// depending on their intended usage within the search pipeline.
/// </remarks>
public static class CompositionRoot
{
    /// <summary>
    /// Registers all infrastructure‑level search dependencies required for
    /// executing establishment search operations. This includes orchestrators,
    /// providers, pipeline steps, and mappers.
    /// </summary>
    /// <param name="services">
    /// The DI service collection to which search dependencies will be added.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> containing all search
    /// infrastructure registrations.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddInfraSearchDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        // ---------------------------------------------------------
        // DbContext factory 
        // ---------------------------------------------------------
        if (!services.Any(serviceDescriptor =>
            serviceDescriptor.ServiceType == typeof(IDbContextFactory<EducationProviderRegistryDbContext>)))
        {
            services.AddDbContextFactory<EducationProviderRegistryDbContext>(options =>
            {
                string connectionString = configuration["eprweb_eprdat_dotnet_db_connection"]
                    ?? throw new InvalidOperationException(
                        "Database connection string not configured.");

                options.UseNpgsql(connectionString)
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors()
                       .LogTo(Console.WriteLine, LogLevel.Information);
            });
        }

        // ---------------------------------------------------------
        // Search orchestrator (trigram)
        // ---------------------------------------------------------
        /// <summary>
        /// Registers the trigram‑based search orchestrator responsible for
        /// executing similarity search queries against the database.
        /// </summary>
        services.TryAddScoped<ISearchOrchestrator<Establishment>, TrigramSearchOrchestrator<Establishment>>();

        // ---------------------------------------------------------
        // Projection builder
        // ---------------------------------------------------------
        /// <summary>
        /// Registers the projection builder used to construct
        /// <see cref="EstablishmentSearchResult"/> projections from
        /// <see cref="Establishment"/> entities.
        /// </summary>
        services.TryAddScoped<ISearchProjectionBuilder<Establishment>,
            EstablishmentSearchProjectionBuilder>();

        // ---------------------------------------------------------
        // Search orchestration metadata
        // ---------------------------------------------------------
        /// <summary>
        /// Registers metadata resolvers used by the search orchestrator to
        /// inspect EF Core entity metadata and optimise SQL generation.
        /// </summary>
        services.AddSingleton(typeof(IEntityMetadataResolver<>), typeof(CachedEntityMetadataResolver<>));
        services.AddScoped(typeof(ISearchOrchestrator<>), typeof(TrigramSearchOrchestrator<>));

        // ---------------------------------------------------------
        // SQL executor
        // ---------------------------------------------------------
        /// <summary>
        /// Registers the SQL executor used by trigram search to execute raw
        /// SQL queries and retrieve results.
        /// </summary>
        services.AddScoped(typeof(ISqlExecutor<>), typeof(SqlExecutor<>));

        // ---------------------------------------------------------
        // Search provider
        // ---------------------------------------------------------
        /// <summary>
        /// Registers the establishment search provider responsible for
        /// orchestrating search execution, projection building, and filter
        /// expression evaluation.
        /// </summary>
        services.TryAddScoped<ISearchProvider<Establishment>>(sp =>
            new EstablishmentsSearchProvider(
                sp.GetRequiredService<IDbContextFactory<EducationProviderRegistryDbContext>>(),
                sp.GetRequiredService<ISearchOrchestrator<Establishment>>(),
                sp.GetRequiredService<ISearchProjectionBuilder<Establishment>>(),
                sp.GetRequiredService<ISearchFilterExpressionsBuilder>(),
                searchColumn: "name" // TODO: move to config
            ));

        // ---------------------------------------------------------
        // Facet provider
        // ---------------------------------------------------------
        /// <summary>
        /// Registers the facet provider responsible for computing facet
        /// aggregations over filtered establishment result sets.
        /// </summary>
        services.TryAddScoped<IFacetProvider, EstablishmentFacetProvider>();

        // ---------------------------------------------------------
        // Pipeline steps
        // ---------------------------------------------------------
        /// <summary>
        /// Registers all search pipeline steps used to transform search
        /// requests into executable queries and map results back into
        /// response models.
        /// </summary>
        services.AddScoped<ISearchPipelineStep, SearchOrderMapStep>();
        services.AddScoped<ISearchPipelineStep, SearchOrderingStep>();

        services.AddScoped<ISearchPipelineStep>(sp =>
            new ParallelMappingStep(
                sp.GetRequiredService<IMapper<Establishment, EstablishmentSearchResult>>()));

        services.AddScoped<ISearchPipelineStep>(sp =>
            new FacetQueryDispatchStep(
                sp.GetRequiredService<IFacetProvider>()));

        services.AddScoped<ISearchPipelineStep, FacetQueryResolverStep>();
        services.AddScoped<ISearchPipelineStep, FacetQueryBuilderStep>();

        // ---------------------------------------------------------
        // Mappers
        // ---------------------------------------------------------
        /// <summary>
        /// Registers mappers used to convert domain entities and pipeline
        /// contexts into search result DTOs.
        /// </summary>
        services.TryAddSingleton<
            IMapper<Establishment, EstablishmentSearchResult>,
            EstablishmentToSearchResultMapper>();

        services.TryAddSingleton<
            IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>>,
            SearchResultsFromContextMapper>();

        return services;
    }

    /// <summary>
    /// Registers all filter‑related dependencies required for constructing
    /// search filter expressions, logical operators, and filter expression
    /// factories.
    /// </summary>
    /// <param name="services">
    /// The DI service collection to which filter dependencies will be added.
    /// </param>
    /// <param name="configuration">
    /// The application configuration used to bind filter expression options.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <c>null</c>.
    /// </exception>
    public static void AddInfraSearchFilterDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        // ---------------------------------------------------------
        // Filter expression formatter
        // ---------------------------------------------------------
        services.TryAddScoped<IFilterExpressionFormatter, DefaultFilterExpressionFormatter>();

        // ---------------------------------------------------------
        // Logical operators
        // ---------------------------------------------------------
        services.TryAddScoped<AndLogicalOperator>();
        services.TryAddScoped<OrLogicalOperator>();

        // ---------------------------------------------------------
        // Filter expressions
        // ---------------------------------------------------------
        services.TryAddScoped<SingleOrMultiValueEqualsExpression>();
        services.TryAddScoped<ISearchFilterExpressionsBuilder, SearchFilterExpressionsBuilder>();

        // ---------------------------------------------------------
        // Filter expression factory
        // ---------------------------------------------------------
        services.TryAddSingleton<ISearchFilterExpressionFactory>(provider =>
        {
            IServiceScope scoped = provider.CreateScope();
            Dictionary<string, Func<ISearchFilterExpression>> map = new()
            {
                ["SingleOrMultiValueEqualsExpression"] = () =>
                    scoped.ServiceProvider.GetRequiredService<SingleOrMultiValueEqualsExpression>()
            };

            return new SearchFilterExpressionFactory(map);
        });

        // ---------------------------------------------------------
        // Filter request mappers
        // ---------------------------------------------------------
        services.TryAddSingleton<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>, SearchRequestFiltersToCoreFiltersMapper>();

        // ---------------------------------------------------------
        // Facet selectors
        // ---------------------------------------------------------
        services.AddSingleton<
            Dictionary<string, Expression<Func<Establishment, object>>>>
            (
                new Dictionary<string, Expression<Func<Establishment, object>>>(StringComparer.OrdinalIgnoreCase)
                {
                    { "establishmenttypeid", e => e.EstablishmentTypeId }
                }
            );

        // ---------------------------------------------------------
        // Logical operator factory
        // ---------------------------------------------------------
        services.TryAddSingleton<ILogicalOperatorFactory>(provider =>
        {
            IServiceScope scoped = provider.CreateScope();
            Dictionary<string, Func<ILogicalOperator>> map = new()
            {
                ["AndLogicalOperator"] = () =>
                    scoped.ServiceProvider.GetRequiredService<AndLogicalOperator>(),
                ["OrLogicalOperator"] = () =>
                    scoped.ServiceProvider.GetRequiredService<OrLogicalOperator>()
            };

            return new LogicalOperatorFactory(map);
        });

        // ---------------------------------------------------------
        // Filter expression map options
        // ---------------------------------------------------------
        services.AddOptions<FilterKeyToFilterExpressionMapOptions>()
            .Bind(configuration.GetSection(nameof(FilterKeyToFilterExpressionMapOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ---------------------------------------------------------
        // Search service adapter
        // ---------------------------------------------------------
        services.AddScoped<
            ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>,
            EstablishmentsSearchServiceAdapter>();
    }
}
