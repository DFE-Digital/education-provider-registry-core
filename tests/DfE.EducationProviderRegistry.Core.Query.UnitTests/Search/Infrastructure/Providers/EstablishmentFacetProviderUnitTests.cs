using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers;

public sealed class EstablishmentFacetProviderUnitTests
{
    [Fact]
    public async Task GetFacetsAsync_Throws_WhenFacetNameUnknown()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        FacetOptions options =
            FacetOptionsTestDouble.Empty();

        FacetProvider provider = new(options);

        // act/assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.GetFacetsAsync(
                context.SearchAggregate,
                "UnknownFacet",
                [],
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetFacetsAsync_ReturnsEmpty_WhenQueryHasNoMatches()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        FacetOptions options =
            FacetOptionsTestDouble.Stub(
                "Type",
                nameof(SearchAggregate.ProviderTypeName),
                nameof(SearchAggregate.ProviderTypeName));

        FacetProvider provider = new(options);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(
                context.SearchAggregate.Where(_ => false),
                "Type",
                [],
                TestContext.Current.CancellationToken);

        // assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetFacetsAsync_GroupsAndCountsCorrectly()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        SearchAggregate a =
            SearchAggregateBuilder.Create()
                .WithProviderName("A School")
                .WithProviderTypeName("primaryType")
                .Build();

        SearchAggregate b =
            SearchAggregateBuilder.Create()
                .WithProviderName("B School")
                .WithProviderTypeName("primaryType")
                .Build();

        SearchAggregate c =
            SearchAggregateBuilder.Create()
                .WithProviderName("C School")
                .WithProviderTypeName("secondaryType")
                .Build();

        context.SearchAggregate.AddRange(a, b, c);

        await context.SaveChangesAsync(
            TestContext.Current.CancellationToken);

        FacetOptions options =
            FacetOptionsTestDouble.Stub(
                "Type",
                nameof(SearchAggregate.ProviderTypeName),
                nameof(SearchAggregate.ProviderTypeName));

        FacetProvider provider = new(options);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(
                context.SearchAggregate,
                "Type",
                [],
                TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(2, results.Count);

        Assert.Equal("primaryType", results[0].Value);
        Assert.Equal("primaryType", results[0].Label);
        Assert.Equal(2, results[0].Count);

        Assert.Equal("secondaryType", results[1].Value);
        Assert.Equal("secondaryType", results[1].Label);
        Assert.Equal(1, results[1].Count);
    }

    [Fact]
    public async Task GetFacetsAsync_OrdersDescendingByCount()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        SearchAggregate a =
            SearchAggregateBuilder.Create()
                .WithProviderName("A School")
                .WithProviderTypeName("X")
                .Build();

        SearchAggregate b =
            SearchAggregateBuilder.Create()
                .WithProviderName("B School")
                .WithProviderTypeName("X")
                .Build();

        SearchAggregate c =
            SearchAggregateBuilder.Create()
                .WithProviderName("C School")
                .WithProviderTypeName("Y")
                .Build();

        context.SearchAggregate.AddRange(a, b, c);

        await context.SaveChangesAsync(
            TestContext.Current.CancellationToken);

        FacetOptions options =
            FacetOptionsTestDouble.Stub(
                "Type",
                nameof(SearchAggregate.ProviderTypeName),
                nameof(SearchAggregate.ProviderTypeName));

        FacetProvider provider = new(options);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(
                context.SearchAggregate,
                "Type",
                [],
                TestContext.Current.CancellationToken);

        // assert
        Assert.Equal("X", results[0].Label);
        Assert.Equal("X", results[0].Value);
        Assert.Equal("Y", results[1].Label);
        Assert.Equal("Y", results[1].Value);
    }

    [Fact]
    public async Task GetFacetsAsync_HandlesNullFacetValues()
    {
        // arrange
        await using EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        SearchAggregate a =
            SearchAggregateBuilder.Create()
                .WithProviderName("A School")
                .WithProviderTypeName(null!)
                .Build();

        SearchAggregate b =
            SearchAggregateBuilder.Create()
                .WithProviderName("B School")
                .WithProviderTypeName("Primary")
                .Build();

        context.SearchAggregate.AddRange(a, b);

        await context.SaveChangesAsync(
            TestContext.Current.CancellationToken);

        FacetOptions options =
            FacetOptionsTestDouble.Stub(
                "Type",
                nameof(SearchAggregate.ProviderTypeName),
                nameof(SearchAggregate.ProviderTypeName));

        FacetProvider provider = new(options);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(
                context.SearchAggregate,
                "Type",
                [],
                TestContext.Current.CancellationToken);

        // assert

        Assert.Single(
            results,
            result =>
                result.Value == "Primary" &&
                result.Label == "Primary");
    }

    // Note: Ef.InMemory does not support translating ToString() in LINQ queries,
    // so we cannot test the scenario where the value property is not a string.
    // In a real database, this would work as expected.
    // This is tested as integration tests with a real database.
}
