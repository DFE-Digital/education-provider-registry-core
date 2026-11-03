using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Factories;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search;

/// <summary>
/// Registers all application‑level and infrastructure‑level dependencies required
/// for trigram‑based search, including orchestrators, filter
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

        services.AddSingleton<
            IMapper<
                IReadOnlyCollection<SearchTermRequest>,
                IReadOnlyCollection<SearchTerm>>,
            SearchRequestTermToSearchRequestMapper>();

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

        services.AddOptions<FacetOptions>()
            .Bind(configuration.GetSection(nameof(FacetOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<FacetOptions>>().Value);

        services.TryAddScoped<ISqlFilterExpressionTranslator<SearchAggregate>,
            SqlFilterExpressionTranslator<SearchAggregate>>();

        services.TryAddScoped<ISearchOrchestrator<SearchAggregate>,
            TrigramSearchOrchestrator<SearchAggregate>>();

        services.TryAddScoped<ISearchProjectionBuilder<SearchAggregate>,
            SearchAggregateProjectionBuilder>();

        services.AddSingleton(typeof(IEntityMetadataResolver<>),
            typeof(CachedEntityMetadataResolver<>));

        services.AddScoped(typeof(ISqlExecutor<>), typeof(SqlExecutor<>));

        services.TryAddScoped<ISearchProvider<SearchAggregate>>(sp =>
            new SearchProvider(
                sp.GetRequiredService<IDbContextFactory<EducationProviderRegistryDbContext>>(),
                sp.GetRequiredService<ISearchOrchestrator<SearchAggregate>>(),
                sp.GetRequiredService<ISearchProjectionBuilder<SearchAggregate>>(),
                sp.GetRequiredService<ISearchFilterExpressionsBuilder<SearchAggregate>>(),
                searchColumn: "name"));

        services.TryAddScoped<IFacetProvider, FacetProvider>();

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
                SearchResults<SearchAggregateResults, SearchFacets>>,
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

        services.TryAddScoped<SenFilter>();

        services.TryAddScoped<ISearchFilterSpecificationFactory<SearchAggregate>>(provider =>
        {
            Dictionary<string, Expression<Func<SearchAggregate, object?>>> equalsFilters =
            new()
            {
                ["ProviderTypeNameFilter"] = aggregate => aggregate.ProviderTypeName,
                ["EstablishmentStatusFilter"] = aggregate => aggregate.StatusName,
                ["EstablishmentEducationPhaseFilter"] = aggregate => aggregate.EducationPhase,
                ["GroupTypeFilter"] = aggregate => aggregate.GroupType,
                ["FurtherEducationTypeFilter"] = aggregate => aggregate.FurtherEducationType,
                ["AdmissionPolicyFilter"] = aggregate => aggregate.Admissions,
                ["BoardingFilter"] = aggregate => aggregate.Boarding,
                ["NurseryFilter"] = aggregate => aggregate.Nursery,
                ["GenderFilter"] = aggregate => aggregate.Gender,
                ["ReligiousCharacterFilter"] = aggregate => aggregate.ReligiousCharacter,
                ["SixthFormFilter"] = aggregate => aggregate.SixthForm,
                ["DioceseFilter"] = aggregate => aggregate.Diocese,
                ["ReligiousEthosFilter"] = aggregate => aggregate.ReligiousEthos,
                ["Section41Filter"] = aggregate => aggregate.Section41,
                ["SpecialClassFilter"] = aggregate => aggregate.SpecialClass
            };

            Dictionary<string, Func<ISearchFilter<SearchAggregate>>> map =
                equalsFilters.ToDictionary(
                    filter => filter.Key,
                    filter => (Func<ISearchFilter<SearchAggregate>>)(() =>
                        new ConvertFilterValuesToStringSearchFilterDecorator<SearchAggregate>(
                            inner: new EqualsFilter(filter.Value))));

            map["OpenedDateFilter"] = () =>
                new ConvertFilterValuesToDateOnlySearchFilterDecorator<SearchAggregate>(
                    inner: new DateRangeFilter(
                        aggregate => aggregate.OpenedDate));

            map["ClosedDateFilter"] = () =>
                new ConvertFilterValuesToDateOnlySearchFilterDecorator<SearchAggregate>(
                    inner: new DateRangeFilter(
                        aggregate => aggregate.ClosedDate));

            map["AgeRangeFilter"] = () =>
                new ConvertFilterValuesToIntegerSearchFilterDecorator<SearchAggregate>(
                    inner: new NumericRangeFilter(
                        aggregate => aggregate.AgeLow,
                        aggregate => aggregate.AgeHigh));

            map["SenFilter"] = () =>
                new ConvertFilterValuesToStringSearchFilterDecorator<SearchAggregate>(
                    inner: provider.GetRequiredService<SenFilter>());

            return new SearchFilterSpecificationFactory<SearchAggregate>(map);
        });

        services.TryAddScoped<
            ISearchFilterExpressionsBuilder<SearchAggregate>,
            SearchFilterExpressionsBuilder<SearchAggregate>>();

        services.TryAddSingleton<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>,
            SearchRequestFiltersToCoreFiltersMapper>();

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
            ISearchServiceAdapter<SearchAggregateResults, SearchFacets>,
            SearchServiceAdapter>();

        // ---------------------------------------------------------
        // Search behaviours
        // ---------------------------------------------------------
        services.AddSingleton(typeof(ExactSearchBehaviour<>));
        services.AddSingleton(typeof(ContainsSearchBehaviour<>));
        services.AddSingleton(typeof(StartsWithSearchBehaviour<>));
        services.AddSingleton(typeof(FuzzySearchBehaviour<>));

        services.AddSingleton<ISearchBehaviourRegistry<SearchAggregate>>((sp) =>
        {
            return new SearchBehaviourRegistry<SearchAggregate>([
                    new("exact", sp.GetRequiredService<ExactSearchBehaviour<SearchAggregate>>()),
                    new("startswith", sp.GetRequiredService<StartsWithSearchBehaviour<SearchAggregate>>()),
                    new("contains", sp.GetRequiredService<ContainsSearchBehaviour<SearchAggregate>>()),
                    new("fuzzy", sp.GetRequiredService<FuzzySearchBehaviour<SearchAggregate>>())
                ]
            );
        });

        // ---------------------------------------------------------
        // Search specification orchestration
        // ---------------------------------------------------------
        services.AddSingleton<IChainingPredicateRegistry<SearchAggregate>>(provider =>
        {
            Dictionary<string, Func<
                ISpecification<SearchAggregate>,
                ISpecification<SearchAggregate>,
                ISpecification<SearchAggregate>>> map =
                    new(StringComparer.OrdinalIgnoreCase)
                    {
                        ["AND"] = (left, right) => left.And(right),
                        ["OR"] = (left, right) => left.Or(right)
                    };

            return new ChainingPredicateRegistry<SearchAggregate>(map);
        });

        services.AddScoped<ISearchIndexFieldSpecificationOrchestrator<SearchAggregate>,
            SearchIndexFieldSpecificationOrchestrator<SearchAggregate>>();

        services.AddScoped<ISearchTermSpecificationOrchestrator<SearchAggregate>,
            SearchTermSpecificationOrchestrator<SearchAggregate>>();

        services.AddScoped(typeof(ISearchQueryProcessor<>), typeof(SearchQueryProcessor<>));
    }
}
