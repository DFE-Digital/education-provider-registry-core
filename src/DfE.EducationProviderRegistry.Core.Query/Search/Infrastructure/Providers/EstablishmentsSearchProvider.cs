using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

public sealed class EstablishmentsSearchProvider : ISearchProvider<Establishment>
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _factory;
    private readonly ISearchOrchestrator<Establishment> _orchestrator;
    private readonly ISearchProjectionBuilder<Establishment> _projectionBuilder;
    private readonly ISearchFilterExpressionsBuilder _searchFilterExpressionsBuilder;
    private readonly string _searchColumn;

    public EstablishmentsSearchProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> factory,
        ISearchOrchestrator<Establishment> orchestrator,
        ISearchProjectionBuilder<Establishment> projectionBuilder,
        ISearchFilterExpressionsBuilder searchFilterExpressionsBuilder,
        string searchColumn)
    {
        _factory = factory;
        _orchestrator = orchestrator;
        _projectionBuilder = projectionBuilder;
        _searchFilterExpressionsBuilder = searchFilterExpressionsBuilder;
        _searchColumn = searchColumn;
    }

    public async Task<IReadOnlyList<Establishment>> GetMatchingIdsAsync(
        string searchTerm,
        int pageSize,
        int offset,
        IReadOnlyList<SearchFilterRequest> filters,
        CancellationToken cancellationToken = default)
    {
        await using EducationProviderRegistryDbContext db =
            await _factory.CreateDbContextAsync(cancellationToken);

        IQueryable<Establishment> baseQuery = _projectionBuilder.Build(db);

        SearchOrchestratorContext context =
            new()
            {
                SearchColumn = _searchColumn,
                SearchColumnConfig = new List<SearchColumnConfig>
                {
                    new(ColumnName: "urn", Strategy: MatchStrategy.ExactPartial),
                    new(ColumnName: "name", Strategy: MatchStrategy.ExactPartialFuzzy),
                    new(ColumnName: "postcode", Strategy: MatchStrategy.ExactPartial, NavigationProperty: "Site"),
                    //new(ColumnName: "group_id", Strategy: MatchStrategy.ExactPartial, NavigationProperty: "EstablishmentGroupMembership")
                },
                SearchTerm = searchTerm,
                PageSize = pageSize,
                Offset = offset,
                Filters = filters
            };

        return await _orchestrator.ExecuteAsync(
            db,
            baseQuery,
            context,
            BuildFilterClause(filters),
            cancellationToken);
    }

    private string BuildFilterClause(IReadOnlyList<SearchFilterRequest> searchFilterRequests)
    {
        string filter = _searchFilterExpressionsBuilder.BuildSearchFilterExpressions(searchFilterRequests);

        return string.IsNullOrWhiteSpace(filter)
            ? string.Empty
            : $" AND {filter}";
    }
}
