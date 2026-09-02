using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

internal sealed class ContainsSpecification<TEntity> : PropertyPathSpecification<TEntity>
{
    private readonly string _value;

    public ContainsSpecification(string propertyPath, string value)
        : base(propertyPath)
    {
        ArgumentNullException.ThrowIfNull(propertyPath);
        ArgumentNullException.ThrowIfNull(value);

        _value = value;
    }

    private static MethodInfo GetIlikeMethod() =>
        typeof(NpgsqlDbFunctionsExtensions)
            .GetMethod(
                nameof(NpgsqlDbFunctionsExtensions.ILike),
                [typeof(DbFunctions), typeof(string), typeof(string)]
            )!;

    protected override Expression BuildExpression(Expression access)
    {
        MemberExpression efFunctions =
        Expression.Property(null, typeof(EF), nameof(EF.Functions));

        ConstantExpression pattern =
            Expression.Constant($"%{_value}%");

        MethodInfo ilikeMethod = GetIlikeMethod();

        return Expression.Call(ilikeMethod, efFunctions, access, pattern);
    }
}
