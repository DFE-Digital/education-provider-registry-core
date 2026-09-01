using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
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

internal sealed class EstablishmentsSearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly ISearchQueryProcessor<Establishment> _searchSpecOrchestrator;
    private readonly ISearchFilterExpressionsBuilder<Establishment> _searchFilterExpressionsBuilder;
    private readonly IFacetAggregator _facetAggregator;
    private readonly IMapper<
        (
            IReadOnlyList<EstablishmentReadModel> Items,
            IReadOnlyList<AggregatedFacetResult> Facets,
            int TotalCount
        ),
        SearchResults<EstablishmentSearchResults, SearchFacets>> _resultsMapper;
    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _filterMapper;

    public EstablishmentsSearchServiceAdapter(
        EducationProviderRegistryDbContext dbContext,
        ISearchQueryProcessor<Establishment> searchSpecOrchestrator,
        ISearchFilterExpressionsBuilder<Establishment> searchFilterExpressionsBuilder,
        IFacetAggregator facetAggregator,
        IMapper<
            (
                IReadOnlyList<EstablishmentReadModel> Items,
                IReadOnlyList<AggregatedFacetResult> Facets,
                int TotalCount
            ),
            SearchResults<EstablishmentSearchResults, SearchFacets>> resultsMapper,
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

    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IQueryable<Establishment> baseQuery = _dbContext.Establishment.AsNoTracking();

        // 1. Map incoming filter requests.
        ReadOnlyCollection<SearchFilterRequest> filterRequests =
            _filterMapper.Map(request.SearchFilterRequests.AsReadOnly());

        // 2. Build the composed filter predicate.
        Expression<Func<Establishment, bool>> filterPredicate =
            _searchFilterExpressionsBuilder.BuildSearchFilterExpression(filterRequests);

        // 3. Apply filter predicate to the base query.
        IQueryable<Establishment> filteredQuery =
            baseQuery.Where(filterPredicate);

        // 4. Apply search-term specification on top of filtered results.
        IQueryable<Establishment> searchResult =
            _searchSpecOrchestrator.ProcessSearch(
                filteredQuery,
                request.SearchTerms);

        // 5. Get the total number of matching establishments before paging.
        int totalCount =
            await searchResult.CountAsync(cancellationToken);

        // 6. Execute paged projection.
        List<EstablishmentReadModel> items =
            await searchResult
                .OrderByDirection(e => e.Name ?? string.Empty, request.SortOrdering.Direction)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .Select(e => new EstablishmentReadModel(
                    int.Parse(e.EstablishmentId.ToString()),
                    e.Urn ?? string.Empty,
                    e.Uid ?? string.Empty,
                    e.Name ?? string.Empty,
                    e.Site.Select(s => s.AddressLine1).FirstOrDefault() ?? string.Empty,
                    e.Site.Select(s => s.AddressLine2).FirstOrDefault() ?? string.Empty,
                    e.Site.Select(s => s.Town).FirstOrDefault() ?? string.Empty,
                    e.Site.Select(s => s.County).FirstOrDefault() ?? string.Empty,
                    e.Site.Select(s => s.Postcode).FirstOrDefault() ?? string.Empty,
                    e.EstablishmentType.Name ?? string.Empty,
                    e.EstablishmentStatus.Name ?? string.Empty,
                    e.EstablishmentGroupMembership
                        .Select(g => g.Group.Name)
                        .FirstOrDefault() ?? string.Empty,
                    e.EstablishmentGroupMembership
                        .Select(g => g.Group.Code)
                        .FirstOrDefault() ?? string.Empty,
                    e.EstablishmentAuthority
                        .Select(a => a.AuthorityName)
                        .FirstOrDefault() ?? string.Empty,
                    e.EstablishmentAuthority
                        .Select(a => a.AuthorityCode)
                        .FirstOrDefault() ?? string.Empty
                ))
                .ToListAsync(cancellationToken);

        // 7. Extract URNs from projected items for facet aggregation.
        IReadOnlyList<string> urns =
            items.Select(entity => entity.Urn)
                .ToList()
                .AsReadOnly();

        // 8. Calculate facet results based on URNs and requested facet keys.
        IReadOnlyList<AggregatedFacetResult> facets =
            await _facetAggregator.CalculateFacetsAsync(
                urns,
                request.Facets,
                cancellationToken);

        // 9. Map results.
        return _resultsMapper.Map(
            (items, facets, totalCount));
    }
}

public record EstablishmentReadModel(
    int Id,
    string Urn,
    string Ukprn,
    string Name,
    string AddressLine1,
    string AddressLine2,
    string? City,
    string? County,
    string? Postcode,
    string Type,
    string Status,
    string GroupName,
    string GroupCode,
    string LocalAuthorityName,
    string LocalAuthorityCode
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
