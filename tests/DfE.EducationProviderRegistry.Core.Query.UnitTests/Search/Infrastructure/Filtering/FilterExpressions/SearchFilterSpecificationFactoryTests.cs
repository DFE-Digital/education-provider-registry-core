using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Factories;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class SearchFilterSpecificationFactoryTests
{
    [Fact]
    public void Constructor_GivenNullFilterRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        Func<SearchFilterSpecificationFactory<EstablishmentAggregate>> construct =
            () => new SearchFilterSpecificationFactory<EstablishmentAggregate>(null!);

        // Act / Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Create_GivenUnknownFilter_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Dictionary<string, Func<ISearchFilter<EstablishmentAggregate>>> filterRegistry = [];

        SearchFilterSpecificationFactory<EstablishmentAggregate> sut =
            new(filterRegistry);

        SearchFilterRequest request = SearchFilterRequestStub.Default();

        // Act / Assert
        const string unknownFilterKey = "UnknownFilter";

        Assert.Throws<ArgumentOutOfRangeException>(
            () => sut.Create(unknownFilterKey, request));
    }

    [Fact]
    public void Create_GivenRegisteredFilter_CallsCreateSpecification()
    {
        // Arrange
        SearchFilterRequest request = SearchFilterRequestStub.Default();

        ISpecification<EstablishmentAggregate> stubSpecification = SpecificationTestDoubles.Create<EstablishmentAggregate>();

        Mock<ISearchFilter<EstablishmentAggregate>> filterMock = new();

        filterMock
            .Setup((filter) => filter.CreateSpecification(request))
            .Returns(stubSpecification);

        int registryInvokeCount = 0;

        Dictionary<string, Func<ISearchFilter<EstablishmentAggregate>>> filterRegistry =
            new()
            {
                ["EstablishmentType"] = () =>
                {
                    registryInvokeCount++;
                    return filterMock.Object;
                }
            };

        SearchFilterSpecificationFactory<EstablishmentAggregate> sut = new(filterRegistry);

        // Act
        ISpecification<EstablishmentAggregate> result =
            sut.Create(
                filterName: "EstablishmentType", request);

        // Assert
        Assert.Same(stubSpecification, result);
        Assert.Equal(1, registryInvokeCount);

        filterMock.Verify(
            filter => filter.CreateSpecification(request),
            Times.Once);
    }
}
