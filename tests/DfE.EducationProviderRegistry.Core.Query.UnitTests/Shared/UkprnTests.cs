using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;

public sealed class UkprnTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   \r\n  \n")]
    public void Constructor_WithInvalidValue_ThrowsArgumentException(string? value)
    {
        // Arrange
        Func<Ukprn> construct = () => new Ukprn(value!);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_WithValue_SetsValue()
    {
        // Arrange
        const string value = "12345678";

        // Act
        Ukprn ukprn = new(value);

        // Assert
        Assert.Equal(value, ukprn.Value);
    }

    [Fact]
    public void Equality_WithSameValue_AreEqual()
    {
        // Arrange
        Ukprn first = new("12345678");
        Ukprn second = new("12345678");

        // Act & Assert
        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_WithDifferentValue_AreNotEqual()
    {
        // Arrange
        Ukprn first = new("12345678");
        Ukprn second = new("87654321");

        // Act & Assert
        Assert.NotEqual(first, second);
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equality_WithNullOther_IsFalse()
    {
        // Arrange
        Ukprn ukprn = new("12345678");
        Ukprn second = null!;

        // Act & Assert
        Assert.False(ukprn.Equals(second));
    }

    [Fact]
    public void Equality_WithObjectReferenceToSameValue_AreEqual()
    {
        // Arrange
        Ukprn ukprn = new("12345678");
        object first = ukprn;
        object second = new Ukprn("12345678");

        // Act & Assert
        Assert.True(first.Equals(second));
    }

    [Fact]
    public void GetHashCode_WithSameValue_ReturnsSameHashCode()
    {
        // Arrange
        Ukprn first = new("12345678");
        Ukprn second = new("12345678");

        // Act
        int firstHashCode = first.GetHashCode();
        int secondHashCode = second.GetHashCode();

        // Assert
        Assert.Equal(firstHashCode, secondHashCode);
    }
}
