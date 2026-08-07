using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

public sealed class EstablishmentFacetProvider : IFacetProvider
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _contextFactory;

    private readonly Dictionary<string, FacetDefinition<Establishment>> _facetDefinitions;

    public EstablishmentFacetProvider(
        IDbContextFactory<EducationProviderRegistryDbContext> contextFactory,
        Dictionary<string, FacetDefinition<Establishment>> facetDefinitions)
    {
        _contextFactory = contextFactory;
        _facetDefinitions = facetDefinitions;
    }

    public async Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IReadOnlyList<string> ids,
        string facetName,
        CancellationToken cancellationToken = default)
    {
        EducationProviderRegistryDbContext? context =
            await _contextFactory.CreateDbContextAsync(cancellationToken) ??
                throw new InvalidOperationException("Failed to create database context.");

        await using (context)
        {
            if (!_facetDefinitions.TryGetValue(facetName, out FacetDefinition<Establishment>? facetDefinition))
            {
                throw new InvalidOperationException($"Unknown facet '{facetName}'.");
            }

            IQueryable<Establishment> filtered =
                context.Establishment.Where(establishment =>
                    ids.Contains(establishment.Urn));

            IQueryable<IGrouping<object, Establishment>> grouped =
                filtered.GroupBy(facetDefinition.Selector);

            IQueryable<dynamic> sqlProjection =
                grouped.Select(groupedFacet => new
                {
                    groupedFacet.Key,

                    Name = facetDefinition.AdditionalValueSelector != null
                        ? groupedFacet.AsQueryable().Select(facetDefinition.AdditionalValueSelector).FirstOrDefault()
                        : null,

                    Count = groupedFacet.LongCount()
                });

            List<dynamic> rawFacetResults =
                await sqlProjection.ToListAsync(cancellationToken);

            List<FacetResult> results =
                [.. rawFacetResults
                    .Select(facetResult => new FacetResult(
                        facetResult.Key?.ToString() ?? string.Empty,
                        facetResult.Name ?? string.Empty,
                        facetResult.Count
                    ))
                    .OrderByDescending(facet => facet.Count)];

            return results;
        }
    }
}
