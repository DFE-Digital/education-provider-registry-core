using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.Base;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

internal sealed class LikeSpecification<TEntity> : ISpecification<TEntity>
{
    private readonly string _propertyPath;
    private readonly string _value;

    public LikeSpecification(string propertyPath, string value)
    {
        ArgumentNullException.ThrowIfNull(propertyPath);
        ArgumentNullException.ThrowIfNull(value);

        _propertyPath = propertyPath;
        _value = value;
    }

    public Expression<Func<TEntity, bool>> ToExpression()
    {
        PropertyPathResolver.ResolvedPath resolved =
            PropertyPathResolver.Resolve<TEntity>(_propertyPath);

        ParameterExpression param = resolved.RootParameter;
        Expression access = resolved.AccessExpression;
        bool isCollection = resolved.IsCollection;
        ParameterExpression? elementParam = resolved.CollectionElementParameter;
        string? collectionName = resolved.CollectionNavigationName;

        Expression efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        MethodInfo ilikeMethod =
            typeof(NpgsqlDbFunctionsExtensions)
                .GetMethod(
                    nameof(NpgsqlDbFunctionsExtensions.ILike),
                    [typeof(DbFunctions), typeof(string), typeof(string)]
                )!;

        Expression pattern = Expression.Constant($"%{_value}%");

        if (!isCollection)
        {
            Expression ilikeCall =
                Expression.Call(ilikeMethod, efFunctions, access, pattern);

            return Expression.Lambda<Func<TEntity, bool>>(ilikeCall, param);
        }

        Expression ilikeElementCall =
            Expression.Call(ilikeMethod, efFunctions, access, pattern);

        LambdaExpression elementLambda = Expression.Lambda(ilikeElementCall, elementParam);
        MemberExpression collectionProp = Expression.PropertyOrField(param, collectionName);

        MethodInfo anyMethod =
            typeof(Enumerable)
                .GetMethods()
                .First(methodInfo =>
                    methodInfo.Name == "Any" &&
                    methodInfo.GetParameters().Length == 2)
                .MakeGenericMethod(elementParam.Type);

        MethodCallExpression anyCall =
            Expression.Call(anyMethod, collectionProp, elementLambda);

        return Expression.Lambda<Func<TEntity, bool>>(anyCall, param);
    }

    public bool IsSatisfiedBy(TEntity input)
        => ToExpression().Compile()(input);
}
