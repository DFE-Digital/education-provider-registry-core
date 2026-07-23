using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class SingleOrMultiValueEqualsExpressionUnitTests
{
    private static SearchFilterRequest Req(params object[] values)
        => new("EstablishmentTypeId", values);

    [Fact]
    public void ToExpression_Throws_WhenRequestIsNull()
    {
        // arrange
        SingleOrMultiValueEqualsExpression<DummyProjection> expression = new();

        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            expression.ToExpression(null!));
    }

    [Fact]
    public void ToExpression_ReturnsSingleEquality()
    {
        // arrange
        SingleOrMultiValueEqualsExpression<DummyProjection> expression = new();
        SearchFilterRequest request = Req("5");
        Expression<Func<DummyProjection, bool>> expr = expression.ToExpression(request);

        // act
        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 5 }));
        Assert.False(compiled(new DummyProjection { EstablishmentTypeId = 6 }));
    }

    [Fact]
    public void ToExpression_ReturnsOrChain_ForMultipleValues()
    {
        // arrange
        SingleOrMultiValueEqualsExpression<DummyProjection> expression = new();
        SearchFilterRequest request = Req("1", "2", "3");
        Expression<Func<DummyProjection, bool>> expr = expression.ToExpression(request);

        // act
        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 1 }));
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 2 }));
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 3 }));
        Assert.False(compiled(new DummyProjection { EstablishmentTypeId = 4 }));
    }

    [Fact]
    public void ToExpression_IgnoresNullAndEmptyValues()
    {
        // arrange
        SingleOrMultiValueEqualsExpression<DummyProjection> expression = new();
        SearchFilterRequest request = Req("1", null!, "", "2");
        Expression<Func<DummyProjection, bool>> expr = expression.ToExpression(request);

        // act
        Func<DummyProjection, bool> compiled = expr.Compile();

        // asserrt
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 1 }));
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 2 }));
        Assert.False(compiled(new DummyProjection { EstablishmentTypeId = 3 }));
    }

    [Fact]
    public void ToExpression_ConvertsValuesToPropertyType()
    {
        // arrange
        SingleOrMultiValueEqualsExpression<DummyProjection> expression = new();
        SearchFilterRequest request = Req("10");
        Expression<Func<DummyProjection, bool>> expr = expression.ToExpression(request);

        // act
        Func<DummyProjection, bool> compiled = expr.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 10 }));
        Assert.False(compiled(new DummyProjection { EstablishmentTypeId = 11 }));
    }
}
