using DfE.EducationProviderRegistry.Core.Query.Search.Application.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Mappers;

public sealed class SearchRequestTermToSearchRequestMapperTests
{
    private readonly SearchRequestTermToSearchRequestMapper _sut = new();

    [Fact]
    public void Map_WhenInputIsNull_ReturnsEmptyCollection()
    {
        // Arrange
        IReadOnlyCollection<SearchTermRequest>? input = null;

        // Act
        IReadOnlyCollection<SearchTerm> result = _sut.Map(input!);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_WhenInputIsEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        IReadOnlyCollection<SearchTermRequest> input = [];

        // Act
        IReadOnlyCollection<SearchTerm> result = _sut.Map(input);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Map_WhenRequestKeyIsInvalid_IgnoresRequest(string? key)
    {
        // Arrange
        IReadOnlyCollection<SearchTermRequest> input =
        [
            new SearchTermRequest(key!, "value")
        ];

        // Act
        IReadOnlyCollection<SearchTerm> result = _sut.Map(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_WhenRequestValueIsNull_ReturnsMappedSearchTerm()
    {
        // Arrange
        IReadOnlyCollection<SearchTermRequest> input =
        [
            new SearchTermRequest("name", null!)
        ];

        // Act
        IReadOnlyCollection<SearchTerm> result = _sut.Map(input);

        // Assert
        SearchTerm term = Assert.Single(result);

        Assert.Equal("name", term.Key);
        Assert.Equal(string.Empty, term.Value);
    }

    [Fact]
    public void Map_WhenInputContainsMixedValidAndInvalidRequests_ReturnsOnlyValidTerms()
    {
        // Arrange
        IReadOnlyCollection<SearchTermRequest> input =
        [
            new SearchTermRequest("name", "john"),
            new SearchTermRequest(string.Empty, "ignored"),
            new SearchTermRequest("postcode", null!),
            null!
        ];

        // Act
        IReadOnlyCollection<SearchTerm> result = _sut.Map(input);

        // Assert
        Assert.Equal(2, result.Count);

        SearchTerm[] terms = result.ToArray();

        Assert.Equal("name", terms[0].Key);
        Assert.Equal("john", terms[0].Value);

        Assert.Equal("postcode", terms[1].Key);
        Assert.Equal(string.Empty, terms[1].Value);
    }
}
