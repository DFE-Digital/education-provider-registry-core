using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Provides trigram‑based search over <see cref="Establishment"/> entities by
/// constructing a projection query, applying filter expressions, and delegating
/// execution to an <see cref="ISearchOrchestrator{TProjection}"/>.
/// </summary>
public sealed class EstablishmentsSearchProvider : ISearchProvider<Establishment>
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _factory;
    private readonly ISearchOrchestrator<Establishment> _orchestrator;
    private readonly ISearchProjectionBuilder<Establishment> _projectionBuilder;
    private readonly ISearchFilterExpressionsBuilder<Establishment> _searchFilterExpressionsBuilder;

    private readonly string _searchColumn;

    /// <summary>
    /// Creates a new search provider for <see cref="Establishment"/> entities.
    /// </summary>
    /// <param name="factory">Factory used to create EF Core database contexts.</param>
    /// <param name="orchestrator">The trigram search orchestrator.</param>
    /// <param name="projectionBuilder">Builds the base LINQ projection for establishments.</param>
    /// <param name="searchFilterExpressionsBuilder">Builds filter expressions from search filters.</param>
    /// <param name="searchColumn">The database column used for trigram similarity search.</param>
    public EstablishmentsSearchProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> factory,
        ISearchOrchestrator<Establishment> orchestrator,
        ISearchProjectionBuilder<Establishment> projectionBuilder,
        ISearchFilterExpressionsBuilder<Establishment> searchFilterExpressionsBuilder,
        string searchColumn)
    {
        _factory = factory;
        _orchestrator = orchestrator;
        _projectionBuilder = projectionBuilder;
        _searchFilterExpressionsBuilder = searchFilterExpressionsBuilder;
        _searchColumn = searchColumn;
    }

    /// <summary>
    /// Executes a trigram similarity search for establishments using the supplied
    /// search term, paging parameters, and filter requests.
    /// </summary>
    /// <param name="searchTerm">The term used for trigram similarity matching.</param>
    /// <param name="pageSize">The maximum number of results to return.</param>
    /// <param name="offset">The number of results to skip.</param>
    /// <param name="filters">Additional filters to apply to the search.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A read‑only list of matching <see cref="Establishment"/> entities.</returns>
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

        Expression<Func<Establishment, bool>> filterExpression =
            _searchFilterExpressionsBuilder.BuildSearchFilterExpression(filters);

        SearchOrchestratorContext<Establishment> context =
            new()
            {
                SearchColumn = _searchColumn,
                SearchTerm = searchTerm,
                PageSize = pageSize,
                Offset = offset,
                Filters = filters,
                FilterExpression = filterExpression
            };

        return await _orchestrator.ExecuteAsync(
            db,
            baseQuery,
            context,
            cancellationToken);
    }
}
