using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.LogicalOperators;

public sealed class OrLogicalOperatorUnitTests
{
    [Fact]
    public void Combine_ReturnsExpressionThatMatchesEitherPredicate()
    {
        // arrange
        OrLogicalOperator<DummyProjection> logicalOperator = new();

        Expression<Func<DummyProjection, bool>> left =
            projection => projection.EstablishmentTypeId == 1;
        Expression<Func<DummyProjection, bool>> right =
            projection => projection.EstablishmentTypeId == 2;

        // act
        Expression<Func<DummyProjection, bool>> combined =
            logicalOperator.Combine(left, right);

        Func<DummyProjection, bool> compiled = combined.Compile();

        // assert
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 1 }));
        Assert.True(compiled(new DummyProjection { EstablishmentTypeId = 2 }));
        Assert.False(compiled(new DummyProjection { EstablishmentTypeId = 3 }));
    }

    [Fact]
    public void Combine_ProducesExpressionWithSingleUnifiedParameter()
    {
        // arrange
        OrLogicalOperator<DummyProjection> logicalOperator = new();

        Expression<Func<DummyProjection, bool>> left =
            projection => projection.EstablishmentTypeId == 1;
        Expression<Func<DummyProjection, bool>> right =
            projection => projection.EstablishmentTypeId == 2;

        // act
        Expression<Func<DummyProjection, bool>> combined =
            logicalOperator.Combine(left, right);

        Assert.Single(combined.Parameters);
        Assert.Equal(typeof(DummyProjection), combined.Parameters[0].Type);
    }

    [Fact]
    public void Combine_ProducesOrElseExpression()
    {
        // arrange
        OrLogicalOperator<DummyProjection> logicalOperator = new();

        Expression<Func<DummyProjection, bool>> left =
            projection => projection.EstablishmentTypeId == 1;

        Expression<Func<DummyProjection, bool>> right =
            projection => projection.EstablishmentTypeId == 2;

        // act
        Expression<Func<DummyProjection, bool>> combined =
            logicalOperator.Combine(left, right);

        // assert
        BinaryExpression body =
            Assert.IsType<BinaryExpression>(combined.Body, exactMatch: false);
        Assert.Equal(ExpressionType.OrElse, body.NodeType);
    }

}
