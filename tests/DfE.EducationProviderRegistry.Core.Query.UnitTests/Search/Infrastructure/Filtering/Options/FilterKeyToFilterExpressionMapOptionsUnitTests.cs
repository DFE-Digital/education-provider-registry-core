using System.ComponentModel.DataAnnotations;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Options.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Options;

public sealed class FilterKeyToFilterExpressionMapOptionsUnitTests
{
    [Fact]
    public void FilterChainingLogicalOperator_WhenNull_FailsValidation()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            FilterKeyToFilterExpressionMapOptionsStub.MissingChainingOperator();

        ValidationContext context = new(options);
        List<ValidationResult> results = [];

        // act
        bool isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        // assert
        Assert.False(isValid);
        Assert.Contains(results, validationResult =>
            validationResult.MemberNames.Contains(nameof(options.FilterChainingLogicalOperator)));
    }

    [Fact]
    public void SearchFilterToExpressionMap_WhenEmpty_FailsValidation()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            FilterKeyToFilterExpressionMapOptionsStub.EmptyMap();

        ValidationContext context = new(options);
        List<ValidationResult> results = [];

        // act
        bool isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        // assert
        Assert.False(isValid);
        Assert.Contains(results, validationResult =>
            validationResult.MemberNames.Contains(nameof(options.SearchFilterToExpressionMap)));
    }

    [Fact]
    public void SearchFilterToExpressionMap_WithOneEntry_PassesValidation()
    {
        // arrange
        FilterKeyToFilterExpressionMapOptions options =
            FilterKeyToFilterExpressionMapOptionsStub.ValidSingle();

        ValidationContext context = new(options);
        List<ValidationResult> results = [];

        // act
        bool isValid = Validator.TryValidateObject(options, context, results, validateAllProperties: true);

        // assert
        Assert.True(isValid);
    }
}
