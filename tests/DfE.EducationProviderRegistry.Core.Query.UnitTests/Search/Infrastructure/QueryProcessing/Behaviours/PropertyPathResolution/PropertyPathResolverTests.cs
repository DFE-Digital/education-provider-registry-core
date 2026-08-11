using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed class PropertyPathResolverTests
{
    [Fact]
    public void Resolve_ScalarPath_ReturnsExpectedRootAndAccess()
    {
        // arrange
        string path = "Name";

        // act
        ResolvedPath result = PropertyPathResolver.Resolve<TestEntity>(path);

        // assert
        Assert.False(result.IsCollection);
        Assert.NotNull(result.RootParameter);
        Assert.Equal("rootParam", result.RootParameter.Name);
        Assert.Equal("Name", result.AccessExpression.GetPropertyName());
        Assert.Null(result.CollectionElementParameter);
        Assert.Null(result.CollectionNavigationName);
    }

    [Fact]
    public void Resolve_NestedScalarPath_ReturnsCorrectAccessExpression()
    {
        // arrange
        string path = "Address.Postcode";

        // act
        ResolvedPath result = PropertyPathResolver.Resolve<TestEntity>(path);

        // assert
        Assert.False(result.IsCollection);
        Assert.Equal("Postcode", result.AccessExpression.GetPropertyName());
        Assert.Equal("Address", result.AccessExpression.GetParent().GetPropertyName());
    }

    [Fact]
    public void Resolve_CollectionPath_ReturnsCollectionMetadata()
    {
        // arrange
        string path = "Sites[].Code";

        // act
        ResolvedPath result = PropertyPathResolver.Resolve<TestEntity>(path);

        // assert
        Assert.True(result.IsCollection);
        Assert.Equal("Sites", result.CollectionNavigationName);
        Assert.NotNull(result.CollectionElementParameter);
        Assert.Equal("elementParam", result.CollectionElementParameter.Name);
        Assert.Equal(typeof(Site), result.CollectionElementParameter.Type);
    }

    [Fact]
    public void Resolve_CollectionPath_ElementAccessIsCorrect()
    {
        // arrange
        string path = "Sites[].Location.Town";

        // act
        ResolvedPath result = PropertyPathResolver.Resolve<TestEntity>(path);

        // assert
        Assert.True(result.IsCollection);
        Assert.Equal("Town", result.AccessExpression.GetPropertyName());
        Assert.Equal("Location", result.AccessExpression.GetParent().GetPropertyName());
    }

    [Fact]
    public void Resolve_InvalidNavigation_Throws()
    {
        // arrange
        string path = "DoesNotExist[].Code";

        // act / assert
        Assert.Throws<InvalidOperationException>(() =>
            PropertyPathResolver.Resolve<TestEntity>(path));
    }

    [Fact]
    public void Resolve_InvalidScalarNavigation_Throws()
    {
        // arrange
        string path = "Address.DoesNotExist";

        // act / assert
        Assert.Throws<ArgumentException>(() =>
            PropertyPathResolver.Resolve<TestEntity>(path));
    }
}
