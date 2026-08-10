using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed class PropertyPathResolverTests
{
    public sealed class TestEntity
    {
        public string Name { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
        public List<Site> Sites { get; set; } = [];
    }

    public sealed class Address
    {
        public string Postcode { get; set; } = string.Empty;
    }

    public sealed class Site
    {
        public string Code { get; set; } = string.Empty;
        public Location Location { get; set; } = new();
    }

    public sealed class Location
    {
        public string Town { get; set; } = string.Empty;
    }

    private static string GetPropertyName(Expression expr)
    {
        return expr switch
        {
            MemberExpression me => me.Member.Name,
            UnaryExpression ue => GetPropertyName(ue.Operand),
            _ => throw new InvalidOperationException($"Unexpected expression type: {expr.GetType().Name}")
        };
    }

    private static Expression GetParent(Expression expr)
    {
        return expr switch
        {
            MemberExpression me => me.Expression!,
            UnaryExpression ue => GetParent(ue.Operand),
            _ => throw new InvalidOperationException($"Unexpected expression type: {expr.GetType().Name}")
        };
    }

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
        Assert.Equal("Name", GetPropertyName(result.AccessExpression));
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
        Assert.Equal("Postcode", GetPropertyName(result.AccessExpression));
        Assert.Equal("Address", GetPropertyName(GetParent(result.AccessExpression)));
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
        Assert.Equal("Town", GetPropertyName(result.AccessExpression));
        Assert.Equal("Location", GetPropertyName(GetParent(result.AccessExpression)));
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
