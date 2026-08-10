using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

internal abstract class PropertyPathSpecification<TEntity> : ISpecification<TEntity>
{
    private readonly string _propertyPath;

    protected PropertyPathSpecification(string propertyPath)
    {
        ArgumentNullException.ThrowIfNull(propertyPath);
        _propertyPath = propertyPath;
    }

    protected abstract Expression BuildExpression(Expression access);

    public Expression<Func<TEntity, bool>> ToExpression()
    {
        ResolvedPath resolved =
            PropertyPathResolver.Resolve<TEntity>(_propertyPath);

        ParameterExpression param = resolved.RootParameter;

        if (!resolved.IsCollection)
        {
            Expression scalarPredicate = BuildExpression(resolved.AccessExpression);
            return Expression.Lambda<Func<TEntity, bool>>(scalarPredicate, param);
        }

        ParameterExpression elementParam = resolved.CollectionElementParameter!;
        Expression elementAccess = resolved.AccessExpression;
        Expression elementPredicate = BuildExpression(elementAccess);

        LambdaExpression elementLambda =
            Expression.Lambda(elementPredicate, elementParam);

        MemberExpression collectionProp =
            Expression.PropertyOrField(param, resolved.CollectionNavigationName!);

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
