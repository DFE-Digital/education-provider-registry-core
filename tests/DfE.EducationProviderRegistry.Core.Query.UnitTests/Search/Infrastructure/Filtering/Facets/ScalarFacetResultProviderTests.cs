using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets.Providers;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets;

public sealed class ScalarFacetResultProviderTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSelectorIsNull()
    {
        // arrange
        Func<FacetProjection, string?> selector = null!;

        // act
        Func<ScalarFacetResultProvider> construct =
            () => new ScalarFacetResultProvider(selector);

        // assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void GetFacets_ThrowsArgumentNullException_WhenProjectionsIsNull()
    {
        // arrange
        ScalarFacetResultProvider sut =
            new(projection => projection.Boarding);

        IEnumerable<FacetProjection> projections = null!;

        // act
        Action act =
            () => sut.GetFacets(
                projections,
                []);

        // assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GetFacets_GroupsValuesAndReturnsCounts()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new() { Boarding = "Boarding school" },
            new() { Boarding = "Boarding school" },
            new() { Boarding = "Does not have boarders" }
        ];

        ScalarFacetResultProvider sut =
            new(projection => projection.Boarding);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                []);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Equal(
            "Boarding school",
            result[0].Value);

        Assert.Equal(
            "Boarding school",
            result[0].Label);

        Assert.Equal(
            2,
            result[0].Count);

        Assert.Equal(
            "Does not have boarders",
            result[1].Value);

        Assert.Equal(
            "Does not have boarders",
            result[1].Label);

        Assert.Equal(
            1,
            result[1].Count);
    }

    [Fact]
    public void GetFacets_ExcludesNullAndEmptyValues()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new() { Boarding = "Boarding school" },
            new() { Boarding = null },
            new() { Boarding = string.Empty }
        ];

        ScalarFacetResultProvider sut =
            new(projection => projection.Boarding);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                []);

        // assert
        FacetResult facetResult =
            Assert.Single(result);

        Assert.Equal(
            "Boarding school",
            facetResult.Value);

        Assert.Equal(
            "Boarding school",
            facetResult.Label);

        Assert.Equal(
            1,
            facetResult.Count);
    }

    [Fact]
    public void GetFacets_ReturnsResultsOrderedByLabel()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new() { Boarding = "Zebra" },
            new() { Boarding = "Alpha" },
            new() { Boarding = "Middle" }
        ];

        ScalarFacetResultProvider sut =
            new(projection => projection.Boarding);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                []);

        // assert
        Assert.Equal(
            ["Alpha", "Middle", "Zebra"],
            result.Select(facetResult =>
                facetResult.Label));
    }

    [Fact]
    public void GetFacets_TreatsValuesAsCaseSensitive()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new() { Boarding = "Boarding" },
            new() { Boarding = "boarding" }
        ];

        ScalarFacetResultProvider sut =
            new(projection => projection.Boarding);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                []);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            facetResult =>
                facetResult.Value == "Boarding" &&
                facetResult.Count == 1);

        Assert.Contains(
            result,
            facetResult =>
                facetResult.Value == "boarding" &&
                facetResult.Count == 1);
    }
}
