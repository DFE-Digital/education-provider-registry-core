using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Contains;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class SenFilterTests
{
    [Fact]
    public void CreateSpecification_Returns_PropertyContainsAnyValuesSpecification()
    {
        // Arrange
        SenFilter sut = new();

        SearchFilterRequest request = new("sen", ["SEN Support"]);

        // Act
        ISpecification<SearchAggregate> specification = sut.CreateSpecification(request);

        // Assert
        Assert.IsType<PropertyContainsAnyValuesSpecification<SearchAggregate, string>>(specification);
    }
}
