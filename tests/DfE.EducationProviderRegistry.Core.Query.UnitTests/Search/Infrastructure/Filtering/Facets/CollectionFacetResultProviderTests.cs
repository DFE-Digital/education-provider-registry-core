using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets.Providers;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets;

public sealed class CollectionFacetResultProviderTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSelectorIsNull()
    {
        // arrange
        Func<FacetProjection, IEnumerable<string>> selector = null!;

        // act
        Func<CollectionFacetResultProvider> construct =
            () => new CollectionFacetResultProvider(selector);

        // assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void GetFacets_ThrowsArgumentNullException_WhenProjectionsIsNull()
    {
        // arrange
        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

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
    public void GetFacets_ThrowsArgumentNullException_WhenFilterValuesIsNull()
    {
        // arrange
        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

        IEnumerable<FacetProjection> projections = [];

        IList<object> filterValues = null!;

        // act
        Action act =
            () => sut.GetFacets(
                projections,
                filterValues);

        // assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GetFacets_FlattensCollectionsAndReturnsCounts()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new()
            {
                Sen =
                [
                    "MLD - Moderate Learning Difficulty",
                    "SEMH - Social, Emotional and Mental Health"
                ]
            },
            new()
            {
                Sen =
                [
                    "MLD - Moderate Learning Difficulty",
                    "ASD - Autistic Spectrum Disorder"
                ]
            }
        ];

        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                []);

        // assert
        Assert.Equal(3, result.Count);

        Assert.Equal(
            "ASD - Autistic Spectrum Disorder",
            result[0].Value);

        Assert.Equal(
            "ASD - Autistic Spectrum Disorder",
            result[0].Label);

        Assert.Equal(
            1,
            result[0].Count);

        Assert.Equal(
            "MLD - Moderate Learning Difficulty",
            result[1].Value);

        Assert.Equal(
            "MLD - Moderate Learning Difficulty",
            result[1].Label);

        Assert.Equal(
            2,
            result[1].Count);

        Assert.Equal(
            "SEMH - Social, Emotional and Mental Health",
            result[2].Value);

        Assert.Equal(
            "SEMH - Social, Emotional and Mental Health",
            result[2].Label);

        Assert.Equal(
            1,
            result[2].Count);
    }

    [Fact]
    public void GetFacets_ExcludesNullAndEmptyValues()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new()
            {
                Sen =
                [
                    "MLD",
                    string.Empty,
                    null!
                ]
            }
        ];

        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                []);

        // assert
        FacetResult facetResult =
            Assert.Single(result);

        Assert.Equal(
            "MLD",
            facetResult.Value);

        Assert.Equal(
            "MLD",
            facetResult.Label);

        Assert.Equal(
            1,
            facetResult.Count);
    }

    [Fact]
    public void GetFacets_ReturnsOnlySelectedValues_WhenFilterValuesAreProvided()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new()
            {
                Sen =
                [
                    "MLD",
                    "SEMH"
                ]
            },
            new()
            {
                Sen =
                [
                    "MLD",
                    "ASD"
                ]
            }
        ];

        IList<object> filterValues =
        [
            "MLD"
        ];

        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                filterValues);

        // assert
        FacetResult facetResult =
            Assert.Single(result);

        Assert.Equal(
            "MLD",
            facetResult.Value);

        Assert.Equal(
            "MLD",
            facetResult.Label);

        Assert.Equal(
            2,
            facetResult.Count);
    }

    [Fact]
    public void GetFacets_ReturnsAllSelectedValues_WhenMultipleFilterValuesAreProvided()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new()
            {
                Sen =
                [
                    "MLD",
                    "SEMH"
                ]
            },
            new()
            {
                Sen =
                [
                    "MLD",
                    "ASD"
                ]
            }
        ];

        IList<object> filterValues =
        [
            "MLD",
            "ASD"
        ];

        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                filterValues);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Equal(
            "ASD",
            result[0].Value);

        Assert.Equal(
            "ASD",
            result[0].Label);

        Assert.Equal(
            1,
            result[0].Count);

        Assert.Equal(
            "MLD",
            result[1].Value);

        Assert.Equal(
            "MLD",
            result[1].Label);

        Assert.Equal(
            2,
            result[1].Count);
    }

    [Fact]
    public void GetFacets_IgnoresNonStringFilterValues()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new()
            {
                Sen =
                [
                    "MLD",
                    "SEMH"
                ]
            }
        ];

        IList<object> filterValues =
        [
            123,
            true
        ];

        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                filterValues);

        // assert
        Assert.Equal(2, result.Count);

        Assert.Equal(
            "MLD",
            result[0].Value);

        Assert.Equal(
            "SEMH",
            result[1].Value);
    }

    [Fact]
    public void GetFacets_ReturnsResultsOrderedByLabel()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new()
            {
                Sen =
                [
                    "SEMH",
                    "ASD",
                    "MLD"
                ]
            }
        ];

        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

        // act
        IReadOnlyList<FacetResult> result =
            sut.GetFacets(
                projections,
                []);

        // assert
        Assert.Equal(
            ["ASD", "MLD", "SEMH"],
            result.Select(facetResult =>
                facetResult.Label));
    }

    [Fact]
    public void GetFacets_TreatsValuesAsCaseSensitive()
    {
        // arrange
        IEnumerable<FacetProjection> projections =
        [
            new()
            {
                Sen =
                [
                    "MLD",
                    "mld"
                ]
            }
        ];

        CollectionFacetResultProvider sut =
            new(projection => projection.Sen ?? []);

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
                facetResult.Value == "MLD" &&
                facetResult.Count == 1);

        Assert.Contains(
            result,
            facetResult =>
                facetResult.Value == "mld" &&
                facetResult.Count == 1);
    }
}
