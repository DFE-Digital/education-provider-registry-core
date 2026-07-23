using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.LogicalOperators;

public sealed class AndLogicalOperatorUnitTests
{
    [Fact]
    public void Combine_ReturnsExpressionThatRequiresBothPredicatesToBeTrue()
    {
        // arrange
        AndLogicalOperator<DummyProjection> logicalOperator = new();

        Expression<Func<DummyProjection, bool>> left =
            projection =>
                projection.EstablishmentTypeId > 0;

        Expression<Func<DummyProjection, bool>> right =
            projection =>
                projection.EstablishmentTypeId < 10;

        Expression<Func<DummyProjection, bool>> combined =
            logicalOperator.Combine(left, right);

        // act
        Func<DummyProjection, bool> compiled = combined.Compile();

        // asesrt
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 5 }));
        Assert.False(compiled(new DummyProjection { EstablishmentTypeId = -1 }));
        Assert.False(compiled(new DummyProjection { EstablishmentTypeId = 10 }));
    }

    [Fact]
    public void Combine_ProducesExpressionWithSingleParameter()
    {
        // arrange
        AndLogicalOperator<DummyProjection> logicalOperator = new();

        Expression<Func<DummyProjection, bool>> left =
            projection =>
                projection.EstablishmentTypeId > 0;

        Expression<Func<DummyProjection, bool>> right =
            projection =>
                projection.EstablishmentTypeId < 10;

        // act
        Expression<Func<DummyProjection, bool>> combined =
            logicalOperator.Combine(left, right);

        // assert
        Assert.Single(combined.Parameters);
        Assert.Equal(typeof(DummyProjection), combined.Parameters[0].Type);
    }

    [Fact]
    public void Combine_ProducesAndAlsoExpression()
    {
        // arrange
        AndLogicalOperator<DummyProjection> logicalOperator = new();

        Expression<Func<DummyProjection, bool>> left =
            projection => projection.EstablishmentTypeId > 0;

        Expression<Func<DummyProjection, bool>> right =
            projection => projection.EstablishmentTypeId < 10;

        // act
        Expression<Func<DummyProjection, bool>> combined =
            logicalOperator.Combine(left, right);

        // assert
        Assert.IsAssignableFrom<BinaryExpression>(combined.Body);
        Assert.Equal(ExpressionType.AndAlso, combined.Body.NodeType);
    }

}
