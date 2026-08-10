using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.Base;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

public sealed class PropertyEqualsSpecification<TEntity> : ISpecification<TEntity>
{
    private readonly string _propertyPath;
    private readonly string _value;

    public PropertyEqualsSpecification(string propertyPath, string value)
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

        ConstantExpression constant = Expression.Constant(_value);

        if (!isCollection)
        {
            BinaryExpression equals = Expression.Equal(access, constant);
            return Expression.Lambda<Func<TEntity, bool>>(equals, param);
        }

        BinaryExpression equalsElement = Expression.Equal(access, constant);
        LambdaExpression elementLambda = Expression.Lambda(equalsElement, elementParam);
        MemberExpression collectionProp = Expression.PropertyOrField(param, collectionName);

        MethodInfo anyMethod =
            typeof(Enumerable)
                .GetMethods()
                .First(methodInfo =>
                    methodInfo.Name == "Any" &&
                    methodInfo.GetParameters().Length == 2)
                .MakeGenericMethod(elementParam.Type);

        MethodCallExpression anyCall = Expression.Call(anyMethod, collectionProp, elementLambda);

        return Expression.Lambda<Func<TEntity, bool>>(anyCall, param);
    }

    public bool IsSatisfiedBy(TEntity input)
        => ToExpression().Compile()(input);
}
