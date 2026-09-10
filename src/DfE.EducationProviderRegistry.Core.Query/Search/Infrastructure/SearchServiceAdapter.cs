using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;

internal sealed class SearchServiceAdapter
    : ISearchServiceAdapter<SearchProviderResults, SearchFacets>
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
        SearchResults<SearchProviderResults, SearchFacets>> _resultsMapper;
    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _filterMapper;

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
            SearchResults<SearchProviderResults, SearchFacets>> resultsMapper,
        IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>> filterMapper)
    {
        _dbContext = dbContext;
        _searchSpecOrchestrator = searchSpecOrchestrator;
        _searchFilterExpressionsBuilder = searchFilterExpressionsBuilder;
        _facetAggregator = facetAggregator;
        _resultsMapper = resultsMapper;
        _filterMapper = filterMapper;
    }

    public async Task<SearchResults<SearchProviderResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IQueryable<SearchAggregate> establishmentsQuery =
            _dbContext.SearchAggregate.AsNoTracking();

        ReadOnlyCollection<SearchFilterRequest> filterRequests =
            _filterMapper.Map(request.SearchFilterRequests.AsReadOnly());

        Expression<Func<SearchAggregate, bool>> filterPredicate =
            _searchFilterExpressionsBuilder.BuildSearchFilterExpression(filterRequests);

        IQueryable<SearchAggregate> filteredEstablishments =
            establishmentsQuery.Where(filterPredicate);

        IQueryable<SearchAggregate> searchResultsQuery =
            _searchSpecOrchestrator.ProcessSearch(
                filteredEstablishments,
                request.SearchTerms);

        int totalCount =
            await searchResultsQuery.CountAsync(cancellationToken);

        string searchTerm = request.SearchTerms.First().Value;
        string searchTermUpper = searchTerm.ToUpperInvariant();

        // 1. Project directly into EstablishmentReadModel.
        List<SearchReadModel> searchResults =
            await searchResultsQuery
                .Select(searchProvider =>
                    new SearchReadModel(
                        Id: searchProvider.ProviderId,
                        Name: searchProvider.ProviderName ?? string.Empty,
                        TypeName: searchProvider.ProviderTypeName ?? string.Empty,
                        TypeId: searchProvider.ProviderTypeId,
                        Address: searchProvider.ProviderAddress ?? string.Empty,
                        LocalAuthorityName: searchProvider.LocalAuthorityName ?? string.Empty,
                        GroupCode: searchProvider.GroupId ?? string.Empty,
                        GroupName: searchProvider.ProviderName ?? string.Empty,
                        ProviderCategory: searchProvider.ProviderCategory ?? string.Empty
                    )
                )
                .ToListAsync(cancellationToken);

        // 2. Define available URN's.
        ReadOnlyCollection<string> urns =
            searchResults
                .Select(entity => entity.Id)
                .ToList()
                .AsReadOnly();

        // 3. Define Facets.
        IReadOnlyList<AggregatedFacetResult> facets =
            await _facetAggregator.CalculateFacetsAsync(
                urns,
                request.Facets,
                cancellationToken);

        return _resultsMapper.Map(
            (searchResults, facets, totalCount));
    }
}

public record SearchReadModel(
    string Id,                  // Urn OR GroupId.
    string Name,                // Establishment OR Group name.
    string TypeName,            // Establishment OR Group type.
    long TypeId,
    string? Address,
    string? LocalAuthorityName,
    string GroupCode,
    string? GroupName,
    string ProviderCategory     // Either "Group" OR "Establishment".
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
