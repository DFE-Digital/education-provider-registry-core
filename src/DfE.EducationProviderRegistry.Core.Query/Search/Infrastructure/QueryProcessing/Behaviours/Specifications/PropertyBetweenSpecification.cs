using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

internal sealed class PropertyBetweenSpecification<TEntity, TValue> : PropertyPathSpecification<TEntity>
    where TValue : struct, IComparable<TValue>
{
    private readonly TValue? _from;
    private readonly TValue? _to;

    public PropertyBetweenSpecification(string propertyPath, TValue? from, TValue? to)
        : base(propertyPath)
    {
        ValidateBounds(from, to);
        _from = from;
        _to = to;
    }

    public PropertyBetweenSpecification(Expression<Func<TEntity, TValue?>> selector, TValue? from, TValue? to)
       : base(selector)
    {
        ValidateBounds(from, to);
        _from = from;
        _to = to;
    }

    protected override Expression BuildExpression(Expression access) =>
        PropertyComparisonExpressionBuilder.Between(access, _from, _to);

    private static void ValidateBounds(TValue? from, TValue? to)
    {
        if (from.HasValue && to.HasValue && Comparer<TValue>.Default.Compare(from.Value, to.Value) > 0)
        {
            throw new ArgumentException("The lower bound must be less than or equal to the upper bound", nameof(from));
        }
    }
}



internal sealed class PropertyGreaterThanOrEqualSpecification<TEntity, TValue> : PropertyComparisonSpecification<TEntity, TValue>
    where TValue : struct, IComparable<TValue>
{
    public PropertyGreaterThanOrEqualSpecification(string propertyPath, TValue bound) :
        base(propertyPath, bound, ExpressionType.GreaterThanOrEqual)
    {
    }

    public PropertyGreaterThanOrEqualSpecification(Expression<Func<TEntity, TValue?>> selector, TValue bound) :
        base(selector, bound, ExpressionType.GreaterThanOrEqual)
    {
    }
}

internal sealed class PropertyLessThanOrEqualSpecification<TEntity, TValue> : PropertyComparisonSpecification<TEntity, TValue>
    where TValue : struct, IComparable<TValue>
{
    public PropertyLessThanOrEqualSpecification(string propertyPath, TValue bound) :
        base(propertyPath, bound, ExpressionType.LessThanOrEqual)
    {
    }

    public PropertyLessThanOrEqualSpecification(Expression<Func<TEntity, TValue?>> selector, TValue bound) :
        base(selector, bound, ExpressionType.LessThanOrEqual)
    {
    }
}



internal abstract class PropertyComparisonSpecification<TEntity, TValue> : PropertyPathSpecification<TEntity>
    where TValue : struct, IComparable<TValue>
{
    private readonly TValue _bound;
    private readonly ExpressionType _operation;

    protected PropertyComparisonSpecification(string propertyPath, TValue bound, ExpressionType operation)
        : base(propertyPath)
    {
        _bound = bound;
        _operation = operation;
    }

    protected PropertyComparisonSpecification(Expression<Func<TEntity, TValue?>> selector, TValue bound, ExpressionType operation)
        : base(selector)
    {
        _bound = bound;
        _operation = operation;
    }

    protected override Expression BuildExpression(Expression access) =>
        PropertyComparisonExpressionBuilder.Compare(access, _bound, _operation);
}

internal static class PropertyComparisonExpressionBuilder
{
    public static Expression Compare<TValue>(Expression access, TValue bound, ExpressionType operation)
        where TValue : struct, IComparable<TValue>
    {
        Expression value = ReadValue<TValue>(access);
        return GuardNull(access, MakeComparison(value, bound, operation));
    }

    public static Expression Between<TValue>(Expression access, TValue? from, TValue? to)
        where TValue : struct, IComparable<TValue>
    {
        Expression value = ReadValue<TValue>(access);
        Expression? predicate = null;

        if (from.HasValue)
        {
            predicate = MakeComparison(value, from.Value, ExpressionType.GreaterThanOrEqual);
        }

        if (to.HasValue)
        {
            Expression upper = MakeComparison(value, to.Value, ExpressionType.LessThanOrEqual);
            predicate = predicate is null ? upper : Expression.AndAlso(predicate, upper);
        }

        return predicate is null ? Expression.Constant(true) : GuardNull(access, predicate);
    }

    private static Expression ReadValue<TValue>(Expression access)
        where TValue : struct, IComparable<TValue>
    {
        Type? underlying = Nullable.GetUnderlyingType(access.Type);
        Type proeprtType = underlying ?? access.Type;
        if (proeprtType != typeof(TValue))
        {
            throw new ArgumentException(
                $"Property type '{access.Type.Name}' does not match comparison type '{typeof(TValue).Name}'",
                nameof(access));
        }

        return underlying is null ? access : Expression.Property(access, nameof(Nullable<int>.Value));
    }

    private static Expression GuardNull(Expression access, Expression predicate) =>
        Nullable.GetUnderlyingType(access.Type) is null
        ? predicate
        : Expression.AndAlso(
            Expression.Property(access, nameof(Nullable<int>.HasValue)), predicate);

    private static Expression MakeComparison<TValue>(Expression value, TValue bound, ExpressionType operation)
        where TValue : struct, IComparable<TValue>
    {
        try
        {
            return Expression.MakeBinary(operation, value, Expression.Constant(bound, typeof(TValue)));
        }
        catch (InvalidOperationException exception)
        {
            throw new NotSupportedException(
                $"Type '{typeof(TValue).Name}' does not support `{operation}` comparisons.", exception);
        }
    }
}
