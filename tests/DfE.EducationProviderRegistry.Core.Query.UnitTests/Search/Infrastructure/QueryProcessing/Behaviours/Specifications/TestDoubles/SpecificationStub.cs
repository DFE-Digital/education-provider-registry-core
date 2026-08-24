using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;

internal sealed class SpecificationStub<TEntity> : PropertyPathSpecification<TEntity>
{
    private readonly string _expected;

    public SpecificationStub(string propertyPath, string expected)
        : base(propertyPath)
    {
        _expected = expected;
    }

    protected override Expression BuildExpression(Expression access) =>
        Expression.Equal(access, Expression.Constant(_expected));
}
