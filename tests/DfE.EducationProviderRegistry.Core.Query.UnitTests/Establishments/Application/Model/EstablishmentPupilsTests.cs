using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentPupilsTests
{
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(10, 5, 5)]
    [InlineData(100, 60, 40)]
    [InlineData(150, 75, 75)]
    public void Constructor_WithValidValues_SetsProperties(int total, int boys, int girls)
    {
        EstablishmentPupils result = new(total, boys, girls);

        Assert.Equal(total, result.Total);
        Assert.Equal(boys, result.Boys);
        Assert.Equal(girls, result.Girls);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeTotal_ThrowsArgumentOutOfRangeException(int total)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new EstablishmentPupils(total, 1, 1));

        Assert.Equal("total", exception.ParamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeBoys_ThrowsArgumentOutOfRangeException(int boys)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new EstablishmentPupils(1, boys, 1));

        Assert.Equal("boys", exception.ParamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeGirls_ThrowsArgumentOutOfRangeException(int girls)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new EstablishmentPupils(1, 1, girls));

        Assert.Equal("girls", exception.ParamName);
    }
}
