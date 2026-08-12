using System.Linq.Expressions;
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
        EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        IDbContextFactory<EducationProviderRegistryDbContext> factory =
            TestDbContextFactory.CreateFactory(context);

        Dictionary<string, FacetDefinition<Establishment>> selectors = [];
        EstablishmentFacetProvider provider = new(factory, selectors);

        // act/assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.GetFacetsAsync(["10001"], "UnknownFacet", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetFacetsAsync_ReturnsEmpty_WhenIdsEmpty()
    {
        EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        IDbContextFactory<EducationProviderRegistryDbContext> factory =
            TestDbContextFactory.CreateFactory(context);

        Dictionary<string, FacetDefinition<Establishment>> selectors =
            new()
            {
                { "Type", new FacetDefinition<Establishment>(e => e.EstablishmentType.Name, e => e.EstablishmentType.Name) }
            };

        EstablishmentFacetProvider provider = new(factory, selectors);

        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync([], "Type", TestContext.Current.CancellationToken);

        Assert.Empty(results);
    }

    private static void ResetBuilders()
    {
        EstablishmentTypeTestBuilder.Reset();
    }

    [Fact]
    public async Task GetFacetsAsync_GroupsAndCountsCorrectly()
    {
        // arrange
        ResetBuilders();

        EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        IDbContextFactory<EducationProviderRegistryDbContext> factory =
            TestDbContextFactory.CreateFactory(context);

        EstablishmentType primaryType =
            EstablishmentTypeTestBuilder.Create(
                establishmentTypeId: 1,
                establishmentFamilyId: 10,
                code: "PRI",
                name: "Primary",
                isSchool: true,
                isGroup: false,
                isEarlyYears: false,
                isFurtherEducation: false);


        EstablishmentType secondaryType =
            EstablishmentTypeTestBuilder.Create(
                establishmentTypeId: 2,
                establishmentFamilyId: 10,
                code: "SEC",
                name: "Secondary",
                isSchool: true,
                isGroup: false,
                isEarlyYears: false,
                isFurtherEducation: false);

        Establishment a = EstablishmentTestBuilder.Create("A", "A School", primaryType);
        Establishment b = EstablishmentTestBuilder.Create("B", "B School", primaryType);
        Establishment c = EstablishmentTestBuilder.Create("C", "C School", secondaryType);

        context.Establishment.AddRange(a, b, c);
        context.SaveChanges();

        Dictionary<string, FacetDefinition<Establishment>> selectors =
            new()
            {
                { "Type", new FacetDefinition<Establishment>(e => e.EstablishmentType.EstablishmentTypeId.ToString(), e => e.EstablishmentType.Name) }
            };

        EstablishmentFacetProvider provider = new(factory, selectors);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(["A", "B", "C"], "Type", TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(2, results.Count);
        Assert.Equal("1", results[0].Value);
        Assert.Equal("Primary", results[0].Label);
        Assert.Equal(2, results[0].Count);
        Assert.Equal("2", results[1].Value);
        Assert.Equal("Secondary", results[1].Label);
        Assert.Equal(1, results[1].Count);
    }

    [Fact]
    public async Task GetFacetsAsync_OrdersDescendingByCount()
    {
        // arrange
        ResetBuilders();

        EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        IDbContextFactory<EducationProviderRegistryDbContext> factory =
            TestDbContextFactory.CreateFactory(context);

        EstablishmentType primaryType =
            EstablishmentTypeTestBuilder.Create(
                establishmentTypeId: 1,
                establishmentFamilyId: 10,
                code: "X",
                name: "X",
                isSchool: true,
                isGroup: false,
                isEarlyYears: false,
                isFurtherEducation: false);

        EstablishmentType secondaryType =
            EstablishmentTypeTestBuilder.Create(
                establishmentTypeId: 2,
                establishmentFamilyId: 10,
                code: "Y",
                name: "Y",
                isSchool: true,
                isGroup: false,
                isEarlyYears: false,
                isFurtherEducation: false);

        Establishment a = EstablishmentTestBuilder.Create("A", "A School", primaryType);
        Establishment b = EstablishmentTestBuilder.Create("B", "B School", primaryType);
        Establishment c = EstablishmentTestBuilder.Create("C", "C School", secondaryType);

        context.Establishment.AddRange(a, b, c);
        context.SaveChanges();

        Dictionary<string, FacetDefinition<Establishment>> selectors =
            new()
            {
                { "Type", new FacetDefinition<Establishment>(e => e.EstablishmentType.EstablishmentTypeId.ToString(), e => e.EstablishmentType.Name) }
            };

        EstablishmentFacetProvider provider = new(factory, selectors);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(
                ["A", "B", "C"], "Type", TestContext.Current.CancellationToken);

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
        ResetBuilders();

        EducationProviderRegistryDbContext context =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        IDbContextFactory<EducationProviderRegistryDbContext> factory =
            TestDbContextFactory.CreateFactory(context);

        EstablishmentType nullType =
            EstablishmentTypeTestBuilder.Create(
                establishmentTypeId: 1,
                establishmentFamilyId: 10,
                code: "NULL",
                name: "PLACEHOLDER",
                isSchool: true,
                isGroup: false,
                isEarlyYears: false,
                isFurtherEducation: false);

        EstablishmentType primaryType =
            EstablishmentTypeTestBuilder.Create(
                establishmentTypeId: 2,
                establishmentFamilyId: 10,
                code: "PRI",
                name: "Primary",
                isSchool: true,
                isGroup: false,
                isEarlyYears: false,
                isFurtherEducation: false);

        Establishment a = EstablishmentTestBuilder.Create("A", "A School", nullType);
        Establishment b = EstablishmentTestBuilder.Create("B", "B School", primaryType);

        context.Establishment.AddRange(a, b);
        context.SaveChanges();

        Dictionary<string, FacetDefinition<Establishment>> selectors =
            new()
            {
                {
                    "Type",
                    new FacetDefinition<Establishment>(
                        establishment => establishment.Urn == "A"
                            ? null!
                            : establishment.EstablishmentTypeId,
                        establishment => establishment.Urn == "A"
                            ? null!
                            : establishment.EstablishmentType.Name!)
                }
            };

        EstablishmentFacetProvider provider = new(factory, selectors);

        // act
        IReadOnlyList<FacetResult> results =
            await provider.GetFacetsAsync(["A", "B"], "Type", TestContext.Current.CancellationToken);

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
