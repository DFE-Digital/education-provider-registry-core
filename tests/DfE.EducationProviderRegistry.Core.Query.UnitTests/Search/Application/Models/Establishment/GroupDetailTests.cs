using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class GroupDetailTests
{
    [Fact]
    public void Constructor_ShouldAssignPropertiesCorrectly()
    {
        // arrange
        GroupDetail detail = new("Mock Group Id","Mock Trust", "TRUST001");

        // assert
        Assert.Equal("Mock Trust", detail.PartOfName);
        Assert.Equal("TRUST001", detail.PartOfCode);
    }

    [Fact]
    public void FactoryMethod_ShouldReturnEquivalentInstance()
    {
        // arrange
        GroupDetail viaCtor = new( "Mock Group Id","Mock Trust", "TRUST001");
        GroupDetail viaFactory = GroupDetail.Create("Mock Group Id","Mock Trust", "TRUST001");

        // assert
        Assert.Equal(viaCtor, viaFactory);
        Assert.NotSame(viaCtor, viaFactory);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenGroupIdIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new GroupDetail(null!, "Mock Trust", "TRUST001"));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPartOfNameIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new GroupDetail("Mock Group Id", null!, "TRUST001"));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPartOfCodeIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new GroupDetail("Mock Group Id", "Mock Trust", null!));
    }

    [Fact]
    public void FactoryMethod_ShouldThrow_WhenPartOfNameIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            GroupDetail.Create("Mock Group Id", null!, "TRUST001"));
    }

    [Fact]
    public void FactoryMethod_ShouldThrow_WhenPartOfCodeIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            GroupDetail.Create("Mock Group Id", "Mock Trust", null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenGroupIdIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            new GroupDetail(invalid, "Mock Trust", "TRUST001"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenPartOfNameIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            new GroupDetail("Mock Group Id", invalid, "TRUST001"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenPartOfCodeIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            new GroupDetail("Mock Group Id", "Mock Trust", invalid));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FactoryMethod_ShouldThrowArgumentException_WhenPartOfNameIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            GroupDetail.Create("Mock Group Id", invalid, "TRUST001"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FactoryMethod_ShouldThrowArgumentException_WhenPartOfCodeIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            GroupDetail.Create("Mock Group Id", "Mock Trust", invalid));
    }
}
