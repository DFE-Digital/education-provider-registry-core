using System.Collections.ObjectModel;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        services.TryAddScoped<ISqlFilterExpressionTranslator<SearchProvider>,
            SqlFilterExpressionTranslator<SearchProvider>>();

        services.TryAddScoped<ISearchOrchestrator<SearchProvider>,
            TrigramSearchOrchestrator<SearchProvider>>();

        services.TryAddScoped<ISearchProjectionBuilder<SearchProvider>,
            EstablishmentSearchProjectionBuilder>();

        services.AddSingleton(typeof(IEntityMetadataResolver<>),
            typeof(CachedEntityMetadataResolver<>));

        services.AddScoped(typeof(ISqlExecutor<>), typeof(SqlExecutor<>));

        services.TryAddScoped<ISearchProvider<SearchProvider>>(sp =>
            new EstablishmentsSearchProvider(
                sp.GetRequiredService<IDbContextFactory<EducationProviderRegistryDbContext>>(),
                sp.GetRequiredService<ISearchOrchestrator<SearchProvider>>(),
                sp.GetRequiredService<ISearchProjectionBuilder<SearchProvider>>(),
                sp.GetRequiredService<ISearchFilterExpressionsBuilder<SearchProvider>>(),
                searchColumn: "name"));

        services.TryAddScoped<IFacetProvider, EstablishmentFacetProvider>();

        // ---------------------------------------------------------
        // Mappers
        // ---------------------------------------------------------
        services.TryAddSingleton<
            IMapper<
                (
                    IReadOnlyList<SearchReadModel> Results,
                    IReadOnlyList<AggregatedFacetResult> Facets,
                    int TotalCount
                ),
                SearchResults<SearchProviderResults, SearchFacets>>,
            SearchResultsFromQueryResultsMapper>();

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

        services
            .AddOptions<SearchConfiguration>()
            .Bind(configuration.GetRequiredSection(nameof(SearchConfiguration)));

        services.TryAddScoped<EstablishmentTypeFilter>();

        services.TryAddScoped<ISearchFilterSpecificationFactory<SearchProvider>>(provider =>
        {
            Dictionary<string, Func<ISearchFilter<SearchProvider>>> map =
                new()
                {
                    ["EstablishmentTypeFilter"] = () =>
                        provider.GetRequiredService<EstablishmentTypeFilter>()
                };

            return new SearchFilterSpecificationFactory<SearchProvider>(map);
        });

        services.TryAddScoped<
            ISearchFilterExpressionsBuilder<SearchProvider>,
            SearchFilterExpressionsBuilder<SearchProvider>>();

        services.TryAddSingleton<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>,
            SearchRequestFiltersToCoreFiltersMapper>();

        services.AddSingleton(
            new Dictionary<string, FacetDefinition<SearchProvider>>(StringComparer.OrdinalIgnoreCase)
            {
                ["establishmenttypeid"] =
                    new FacetDefinition<SearchProvider>(
                        searchProvider => searchProvider.ProviderId,
                        searchProvider => searchProvider.ProviderName)
            });

        services.AddScoped<IFacetAggregator, FacetAggregator>();

        // ---------------------------------------------------------
        // Logical operator factory
        // ---------------------------------------------------------
        services.AddOptions<FilterKeyToFilterExpressionMapOptions>()
            .Configure<IConfiguration>((settings, cfg) =>
                cfg.GetSection("FilterKeyToFilterExpressionMapOptions").Bind(settings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<
            ISearchServiceAdapter<SearchProviderResults, SearchFacets>,
            EstablishmentsSearchServiceAdapter>();

        // ---------------------------------------------------------
        // Search behaviours
        // ---------------------------------------------------------
        services.AddSingleton(typeof(ExactSearchBehaviour<>));
        services.AddSingleton(typeof(ContainsSearchBehaviour<>));
        services.AddSingleton(typeof(StartsWithSearchBehaviour<>));
        services.AddSingleton(typeof(FuzzySearchBehaviour<>));

        services.AddSingleton<ISearchBehaviourRegistry<SearchProvider>>((sp) =>
        {
            return new SearchBehaviourRegistry<SearchProvider>([
                    new("exact", sp.GetRequiredService<ExactSearchBehaviour<SearchProvider>>()),
                    new("startswith", sp.GetRequiredService<StartsWithSearchBehaviour<SearchProvider>>()),
                    new("contains", sp.GetRequiredService<ContainsSearchBehaviour<SearchProvider>>()),
                    new("fuzzy", sp.GetRequiredService<FuzzySearchBehaviour<SearchProvider>>())
                ]
            );
        });

        // ---------------------------------------------------------
        // Search specification orchestration
        // ---------------------------------------------------------
        services.AddSingleton<IChainingPredicateRegistry<SearchProvider>>(provider =>
        {
            Dictionary<string, Func<
                ISpecification<SearchProvider>,
                ISpecification<SearchProvider>,
                ISpecification<SearchProvider>>> map =
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["AND"] = (left, right) => left.And(right),
                        ["OR"] = (left, right) => left.Or(right)
                    };

            return new ChainingPredicateRegistry<SearchProvider>(map);
        });

        services.AddScoped<ISearchIndexFieldSpecificationOrchestrator<SearchProvider>,
            SearchIndexFieldSpecificationOrchestrator<SearchProvider>>();

        services.AddScoped<ISearchTermSpecificationOrchestrator<SearchProvider>,
            SearchTermSpecificationOrchestrator<SearchProvider>>();

        services.AddScoped(typeof(ISearchQueryProcessor<>), typeof(SearchQueryProcessor<>));
    }
}
