using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

internal sealed class PropertyEqualsSpecification<TEntity> : PropertyPathSpecification<TEntity>
{
    private readonly string _value;

    public PropertyEqualsSpecification(string propertyPath, string value)
        : base(propertyPath)
    {
        ArgumentNullException.ThrowIfNull(propertyPath);
        ArgumentNullException.ThrowIfNull(value);

        _value = value;
    }

    protected override Expression BuildExpression(Expression access)
    {
        ConstantExpression constant = Expression.Constant(_value);
        return Expression.Equal(access, constant);
    }
}
