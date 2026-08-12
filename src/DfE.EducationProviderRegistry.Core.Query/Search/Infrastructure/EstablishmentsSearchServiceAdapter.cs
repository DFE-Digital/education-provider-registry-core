using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
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
        (IReadOnlyList<EstablishmentReadModel>, IReadOnlyList<AggregatedFacetResult>),
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
            (IReadOnlyList<EstablishmentReadModel>, IReadOnlyList<AggregatedFacetResult>),
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

        ReadOnlyCollection<SearchFilterRequest> filterRequests =
            _filterMapper.Map(request.SearchFilterRequests.AsReadOnly());

        IQueryable<Establishment> searchResult =
            _searchSpecOrchestrator.ProcessSearch(baseQuery, request.SearchTerms);

        List<EstablishmentReadModel> items = null!;

        items = await searchResult
            .OrderBy(e => e.Name)
            .Skip(request.Offset)
            .Take(request.PageSize)
            .Select(e => new EstablishmentReadModel(
                int.Parse(e.EstablishmentId.ToString()),
                e.Urn,
                e.Uid,
                e.Name,
                e.Site.Select(s => s.AddressLine1).FirstOrDefault(),
                e.Site.Select(s => s.Town).FirstOrDefault(),
                e.Site.Select(s => s.County).FirstOrDefault(),
                e.Site.Select(s => s.Postcode).FirstOrDefault(),
                e.EstablishmentType.Name ?? string.Empty,
                e.EstablishmentStatus.Name ?? string.Empty,
                e.EstablishmentGroupMembership.Select(g => g.Group.Name).FirstOrDefault(),
                e.EstablishmentGroupMembership.Select(g => g.Group.Code).FirstOrDefault(),
                e.EstablishmentAuthority.Select(a => a.AuthorityName).FirstOrDefault(),
                e.EstablishmentAuthority.Select(a => a.AuthorityCode).FirstOrDefault()
             ))
            .ToListAsync(cancellationToken);

        IReadOnlyList<string> urns = items.Select(e => e.Urn).ToList().AsReadOnly();

        IReadOnlyList<AggregatedFacetResult> facets =
            await _facetAggregator.CalculateFacetsAsync(
                urns,
                request.Facets,
                cancellationToken);

        return _resultsMapper.Map((items, facets));
    }
}

public interface IFacetAggregator
{
    Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IReadOnlyList<string> urns,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken);
}

public class FacetAggregationStep : IFacetAggregator
{
    private readonly IFacetProvider _facetProvider;

    public FacetAggregationStep(IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    public async Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IReadOnlyList<string> urns,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken)
    {
        if (requestedFacets == null || !requestedFacets.Any())
        {
            return Array.Empty<AggregatedFacetResult>();
        }

        List<AggregatedFacetResult> aggregatedFacetResults = [];

        foreach (string facetKey in requestedFacets.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            IReadOnlyList<FacetResult> facetResults =
                await _facetProvider.GetFacetsAsync(urns, facetKey, cancellationToken);

            aggregatedFacetResults.Add(new AggregatedFacetResult(facetKey, facetResults));
        }

        return aggregatedFacetResults.AsReadOnly();
    }
}

public record AggregatedFacetResult(string FacetName, IReadOnlyCollection<FacetResult> Values);

public record EstablishmentReadModel(
    int Id,
    string Urn,
    string Ukprn,
    string Name,
    string AddressLine1,
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
