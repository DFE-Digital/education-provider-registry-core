using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentFreeSchoolMealsTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10, 25.5)]
    [InlineData(100, 100)]
    public void Constructor_WithValidValues_SetsProperties(int number, decimal percentage)
    {
        EstablishmentFreeSchoolMeals result = new(number, percentage);

        Assert.Equal(number, result.Number);
        Assert.Equal(percentage, result.Percentage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeNumber_ThrowsArgumentOutOfRangeException(int number)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EstablishmentFreeSchoolMeals(number, 50));

        Assert.Equal("number", exception.ParamName);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-50)]
    public void Constructor_WithNegativePercentage_ThrowsArgumentOutOfRangeException(decimal percentage)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EstablishmentFreeSchoolMeals(10, percentage));

        Assert.Equal("percentage", exception.ParamName);
    }
}
