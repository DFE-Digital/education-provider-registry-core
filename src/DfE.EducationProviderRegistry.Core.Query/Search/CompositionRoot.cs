using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;
using DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search;

/// <summary>
/// Registers all application‑level and infrastructure‑level dependencies required
/// for trigram‑based establishment search, including orchestrators, filter
/// expression builders, facet providers, pipeline steps, and mappers.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers application‑layer search dependencies, including configuration
    /// binding and the <see cref="SearchUseCase"/>.
    /// </summary>
    public static IServiceCollection AddApplicationSearchDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddOptions<SearchCriteria>()
            .Bind(configuration.GetSection(nameof(SearchCriteria)));

        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<SearchCriteria>>().Value);

        services.AddScoped<
            IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>,
            SearchUseCase>();

        return services;
    }

    /// <summary>
    /// Registers infrastructure‑layer dependencies required for trigram search,
    /// including EF Core metadata resolvers, orchestrators, SQL executors,
    /// projection builders, facet providers, and pipeline steps.
    /// </summary>
    public static IServiceCollection AddInfraSearchDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<ISqlFilterExpressionTranslator<Establishment>,
            SqlFilterExpressionTranslator<Establishment>>();

        services.TryAddScoped<ISearchOrchestrator<Establishment>,
            TrigramSearchOrchestrator<Establishment>>();

        services.TryAddScoped<ISearchProjectionBuilder<Establishment>,
            EstablishmentSearchProjectionBuilder>();

        services.AddSingleton(typeof(IEntityMetadataResolver<>),
            typeof(CachedEntityMetadataResolver<>));

        services.AddScoped(typeof(ISqlExecutor<>), typeof(SqlExecutor<>));

        services.TryAddScoped<ISearchProvider<Establishment>>(sp =>
            new EstablishmentsSearchProvider(
                sp.GetRequiredService<IDbContextFactory<EducationProviderRegistryDbContext>>(),
                sp.GetRequiredService<ISearchOrchestrator<Establishment>>(),
                sp.GetRequiredService<ISearchProjectionBuilder<Establishment>>(),
                sp.GetRequiredService<ISearchFilterExpressionsBuilder<Establishment>>(),
                searchColumn: "name"));

        services.TryAddScoped<IFacetProvider, EstablishmentFacetProvider>();

        // Pipeline steps
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

        // Mappers
        services.TryAddSingleton<
            IMapper<Establishment, EstablishmentSearchResult>,
            EstablishmentToSearchResultMapper>();

        services.TryAddSingleton<
            IMapper<SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>>,
            SearchResultsFromContextMapper>();

        return services;
    }

    /// <summary>
    /// Registers filtering‑layer dependencies, including logical operators,
    /// filter expression factories, filter expression builders, facet selectors,
    /// and filter‑mapping options.
    /// </summary>
    public static void AddInfraSearchFilterDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<EstablishmentTypeFilter>();

        services.TryAddScoped<ISearchFilterSpecificationFactory<Establishment>>(provider =>
        {
            Dictionary<string, Func<ISearchFilter<Establishment>>> map =
                new()
                {
                    ["EstablishmentTypeFilter"] = () =>
                        provider.GetRequiredService<EstablishmentTypeFilter>()
                };

            return new SearchFilterSpecificationFactory<Establishment>(map);
        });

        services.TryAddScoped<
            ISearchFilterExpressionsBuilder<Establishment>,
            SearchFilterExpressionsBuilder<Establishment>>();

        services.TryAddSingleton<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>,
            SearchRequestFiltersToCoreFiltersMapper>();

        services.AddSingleton(
            new Dictionary<string, FacetDefinition<Establishment>>(StringComparer.OrdinalIgnoreCase)
            {
                ["establishmenttypeid"] =
                    new FacetDefinition<Establishment>(
                        establishment => establishment.EstablishmentTypeId,
                        establishment => establishment.EstablishmentType.Name)
            });

        services.AddOptions<FilterKeyToFilterExpressionMapOptions>()
            .Configure<IConfiguration>((settings, cfg) =>
                cfg.GetSection("FilterKeyToFilterExpressionMapOptions").Bind(settings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<
            ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>,
            EstablishmentsSearchServiceAdapter>();
    }
}
