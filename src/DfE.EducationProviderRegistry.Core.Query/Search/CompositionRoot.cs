using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours;
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
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search;

public static class CompositionRoot
{
    public static IServiceCollection AddApplicationSearchDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Bind SearchCriteria configuration from appsettings.json.
        services
            .AddOptions<SearchCriteria>()
            .Bind(configuration.GetSection(nameof(SearchCriteria)));

        // Register the resolved SearchCriteria instance for direct injection.
        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<SearchCriteria>>().Value);

        services.AddScoped<
            IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>,
            SearchUseCase>();

        return services;
    }

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
        // ---------------------------------------------------------
        // Search service adapter
        // ---------------------------------------------------------
        services.AddScoped<
            ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>,
            EstablishmentsSearchServiceAdapter>();

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
        services.TryAddScoped<IFacetProvider, EstablishmentFacetProvider>();

        // ---------------------------------------------------------
        // Pipeline steps
        // ---------------------------------------------------------

        services.AddScoped<IEvaluationHandler<SearchPipelineContext>, SearchOrderMapStep>();
        services.AddScoped<IEvaluationHandler<SearchPipelineContext>, SearchOrderingStep>();
        services.AddScoped<IEvaluationHandler<SearchPipelineContext>, ParallelMappingStep>();
        services.AddScoped<IEvaluationHandler<SearchPipelineContext>, FacetQueryDispatchStep>();
        services.AddScoped<IEvaluationHandler<SearchPipelineContext>, FacetQueryResolverStep>();
        services.AddScoped<IEvaluationHandler<SearchPipelineContext>, FacetQueryBuilderStep>();

        services.AddScoped<IEvaluator<SearchPipelineContext>>((sp) =>
        {
            IEnumerable<IEvaluationHandler<SearchPipelineContext>> handlers = sp.GetServices<IEvaluationHandler<SearchPipelineContext>>();

            return new PipelineEvaluator(handlers);
        });

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
        services.AddSingleton
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
        // Search specification orchestration
        // ---------------------------------------------------------
        services.AddSingleton(typeof(SearchBehaviourRegistry<>));

        // Predicate map (closed generic)
        services.AddSingleton(
            new Dictionary<string, Func<
                ISpecification<Establishment>,
                ISpecification<Establishment>,
                ISpecification<Establishment>>>
            {
                ["AND"] = (left, right) => left.And(right),
                ["OR"] = (left, right) => left.Or(right)
            });

        services.AddSingleton<ChainingPredicateRegistry<Establishment>>();

        services.AddScoped<ISearchIndexFieldSpecificationOrchestrator<Establishment>,
            SearchIndexFieldSpecificationOrchestrator<Establishment>>();

        services.AddScoped<ISearchTermSpecificationOrchestrator<Establishment>,
            SearchTermSpecificationOrchestrator<Establishment>>();
    }
}
