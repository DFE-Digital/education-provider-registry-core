using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers;

public sealed class EstablishmentFacetProviderUnitTests
{
    [Fact]
    public async Task GetFacetsAsync_Throws_WhenFacetNameUnknown()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        Dictionary<object, FacetDefinition<SearchAggregate>> selectors = [];
        FacetProvider provider = new(selectors);

        // act/assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.GetFacetsAsync(context.SearchAggregate, "UnknownFacet", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetFacetsAsync_ReturnsEmpty_WhenQueryHasNoMatches()
    {
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        Dictionary<object, FacetDefinition<SearchAggregate>> selectors =
            new()
            {
                { "Type", new FacetDefinition<SearchAggregate>(e => e.ProviderName!, e => e.ProviderTypeName!) }
            };

        FacetProvider provider = new(selectors);

        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(context.SearchAggregate.Where(_ => false), "Type", TestContext.Current.CancellationToken);

        Assert.Empty(results);
    }

    [Fact]
    public async Task GetFacetsAsync_GroupsAndCountsCorrectly()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();


        SearchAggregate a = SearchAggregateTestBuilder.Create("A", "A School", "primaryType", 1);
        SearchAggregate b = SearchAggregateTestBuilder.Create("B", "B School", "primaryType", 1);
        SearchAggregate c = SearchAggregateTestBuilder.Create("C", "C School", "secondaryType", 2);

        context.SearchAggregate.AddRange(a, b, c);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Dictionary<object, FacetDefinition<SearchAggregate>> selectors =
            new()
            {
                { "Type", new FacetDefinition<SearchAggregate>(
                    e => e.ProviderTypeId!,
                    e => e.ProviderTypeName!) }
            };

        FacetProvider provider = new(selectors);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(context.SearchAggregate, "Type", TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(2, results.Count);
        Assert.Equal("1", results[0].Value);
        Assert.Equal("primaryType", results[0].Label);
        Assert.Equal(2, results[0].Count);
        Assert.Equal("2", results[1].Value);
        Assert.Equal("secondaryType", results[1].Label);
        Assert.Equal(1, results[1].Count);
    }

    [Fact]
    public async Task GetFacetsAsync_OrdersDescendingByCount()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        SearchAggregate a = SearchAggregateTestBuilder.Create("A", "A School", "X", 1);
        SearchAggregate b = SearchAggregateTestBuilder.Create("B", "B School", "X", 1);
        SearchAggregate c = SearchAggregateTestBuilder.Create("C", "C School", "Y", 2);

        context.SearchAggregate.AddRange(a, b, c);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Dictionary<object, FacetDefinition<SearchAggregate>> selectors =
            new()
            {
                { "Type", new FacetDefinition<SearchAggregate>(
                    e => e.ProviderTypeId!,
                    e => e.ProviderTypeName!) }
            };

        FacetProvider provider = new(selectors);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(context.SearchAggregate, "Type", TestContext.Current.CancellationToken);

        // assert
        Assert.Equal("1", results[0].Value);
        Assert.Equal("2", results[1].Value);
        Assert.Equal("X", results[0].Label);
        Assert.Equal("Y", results[1].Label);
    }

    [Fact]
    public async Task GetFacetsAsync_HandlesNullFacetValues()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        SearchAggregate a = SearchAggregateTestBuilder.Create("A", "A School", null!, 1);
        SearchAggregate b = SearchAggregateTestBuilder.Create("B", "B School", "Primary", 2);

        context.SearchAggregate.AddRange(a, b);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Dictionary<object, FacetDefinition<SearchAggregate>> selectors =
            new()
            {
                {
                    "Type",
                    new FacetDefinition<SearchAggregate>(
                        searchResult => searchResult.ProviderId == "A"
                            ? null!
                            : searchResult.ProviderTypeId!,
                        searchResult => searchResult.ProviderId == "A"
                            ? null!
                            : searchResult.ProviderTypeName!)
                }
            };

        FacetProvider provider = new(selectors);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(context.SearchAggregate, "Type", TestContext.Current.CancellationToken);

        // assert
        Assert.Contains(
            results,
            r => r.Value == string.Empty &&
                 r.Label == string.Empty);

        Assert.Contains(
            results,
            r => r.Value == "2" &&
                 r.Label == "Primary");
    }
}
