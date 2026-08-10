using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.Base;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

internal sealed class TrigramFuzzySpecification<TEntity> : ISpecification<TEntity>
{
    private readonly string _propertyPath;
    private readonly string _value;
    private readonly double _threshold;

    public TrigramFuzzySpecification(string propertyPath, string value, double threshold)
    {
        ArgumentNullException.ThrowIfNull(propertyPath);
        ArgumentNullException.ThrowIfNull(value);

        _propertyPath = propertyPath;
        _value = value;
        _threshold = threshold;
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

        Expression efFunctions =
            Expression.Property(null, typeof(EF), nameof(EF.Functions));

        MethodInfo similarityMethod =
            typeof(NpgsqlTrigramsDbFunctionsExtensions)
                .GetMethod(
                    nameof(NpgsqlTrigramsDbFunctionsExtensions.TrigramsWordSimilarity),
                    [typeof(DbFunctions), typeof(string), typeof(string)]
                )!;

        if (!isCollection)
        {
            Expression similarityCall =
                Expression.Call(
                    similarityMethod,
                    efFunctions,
                    Expression.Constant(_value),
                    access);

            Expression thresholdExpr = Expression.Constant(_threshold);
            Expression comparison = Expression.GreaterThanOrEqual(similarityCall, thresholdExpr);

            return Expression.Lambda<Func<TEntity, bool>>(comparison, param);
        }

        Expression similarityElementCall =
            Expression.Call(
                similarityMethod,
                efFunctions,
                Expression.Constant(_value),
                access);

        Expression thresholdElementExpr = Expression.Constant(_threshold);
        Expression comparisonElement =
            Expression.GreaterThanOrEqual(similarityElementCall, thresholdElementExpr);

        LambdaExpression elementLambda = Expression.Lambda(comparisonElement, elementParam);
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
