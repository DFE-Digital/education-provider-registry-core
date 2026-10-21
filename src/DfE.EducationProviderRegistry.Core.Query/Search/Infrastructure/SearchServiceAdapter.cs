using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;

public sealed class SearchServiceAdapter
    : ISearchServiceAdapter<SearchAggregateResults, SearchFacets>
{
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly ISearchQueryProcessor<SearchAggregate> _searchSpecOrchestrator;
    private readonly ISearchFilterExpressionsBuilder<SearchAggregate> _searchFilterExpressionsBuilder;
    private readonly IFacetAggregator _facetAggregator;
    private readonly IMapper<
        (
            IReadOnlyList<SearchReadModel> Items,
            IReadOnlyList<AggregatedFacetResult> Facets,
            int TotalCount
        ),
        SearchResults<SearchAggregateResults, SearchFacets>> _resultsMapper;

    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _filterMapper;

    private readonly IMapper<
        IReadOnlyCollection<SearchTermRequest>,
        IReadOnlyCollection<SearchTerm>> _termMapper;

    public SearchServiceAdapter(
        EducationProviderRegistryDbContext dbContext,
        ISearchQueryProcessor<SearchAggregate> searchSpecOrchestrator,
        ISearchFilterExpressionsBuilder<SearchAggregate> searchFilterExpressionsBuilder,
        IFacetAggregator facetAggregator,
        IMapper<
            (
                IReadOnlyList<SearchReadModel> Items,
                IReadOnlyList<AggregatedFacetResult> Facets,
                int TotalCount
            ),
            SearchResults<SearchAggregateResults, SearchFacets>> resultsMapper,
        IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>> filterMapper,
        IMapper<
            IReadOnlyCollection<SearchTermRequest>,
            IReadOnlyCollection<SearchTerm>> termMapper)
    {
        _dbContext = dbContext;
        _searchSpecOrchestrator = searchSpecOrchestrator;
        _searchFilterExpressionsBuilder = searchFilterExpressionsBuilder;
        _facetAggregator = facetAggregator;
        _resultsMapper = resultsMapper;
        _filterMapper = filterMapper;
        _termMapper = termMapper;
    }

    public async Task<SearchResults<SearchAggregateResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IQueryable<SearchAggregate> searchQuery =
            _dbContext.SearchAggregate.AsNoTracking();

        ReadOnlyCollection<SearchFilterRequest> filterRequests =
            _filterMapper.Map(request.SearchFilterRequests.AsReadOnly());

        Expression<Func<SearchAggregate, bool>> filterPredicate =
            _searchFilterExpressionsBuilder.BuildSearchFilterExpression(filterRequests);

        IQueryable<SearchAggregate> filteredResults =
            searchQuery.Where(filterPredicate);

        IQueryable<SearchAggregate> searchResultsQuery =
            _searchSpecOrchestrator.ProcessSearch(
                filteredResults,
                _termMapper.Map(request.SearchTerms));

        int totalCount =
            await searchResultsQuery.CountAsync(cancellationToken);

        // count facets across all matches, independently of the current page
        IReadOnlyList<AggregatedFacetResult> facets =
            await _facetAggregator.CalculateFacetsAsync(
                searchResultsQuery,
                request.Facets,
                cancellationToken);

        // Apply pagination only to the dsplayed results
        List<SearchReadModel> searchResults =
            await searchResultsQuery
                .OrderByDirection(t => t.ProviderName, request.SortOrdering.Direction)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .Select(searchProvider =>
                    new SearchReadModel(
                        Id: searchProvider.ProviderId,
                        Name: searchProvider.ProviderName ?? string.Empty,
                        TypeName: searchProvider.ProviderTypeName ?? string.Empty,
                        TypeId: searchProvider.ProviderTypeId,
                        Address: searchProvider.ProviderAddress ?? string.Empty,
                        LocalAuthorityName: searchProvider.LocalAuthorityName ?? string.Empty,
                        GroupUid: searchProvider.GroupUid,
                        GroupName: searchProvider.GroupName,
                        ProviderCategory: searchProvider.ProviderCategory ?? string.Empty,
                        EstablishmentStatusCode: searchProvider.EstablishmentStatus,
                        GroupStatusCode: searchProvider.GroupStatus,
                        AcademyCount: searchProvider.AcademyCounts ?? 0
                    )
                )
                .ToListAsync(cancellationToken);


        return _resultsMapper.Map(
            (searchResults, facets, totalCount));
    }
}

public record SearchReadModel(
    string Id,                  // Urn OR GroupId.
    string Name,                // SearchAggregate OR Group name.
    string TypeName,            // SearchAggregate OR Group type.
    long? TypeId,
    string? Address,
    string? LocalAuthorityName,
    string? GroupUid,
    string? GroupName,
    string ProviderCategory,     // Either "Group" OR "SearchAggregate".
    int? EstablishmentStatusCode,
    int? GroupStatusCode,
    int AcademyCount
);

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> OrderByDirection<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        SortDirection sortDirection)
    {
        return sortDirection == SortDirection.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
