using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.DesignPatterns.Specification;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours.Specifications;

internal sealed class TrigramFuzzySpecification<TEntity> : ISpecification<TEntity>
{
    private readonly string _propertyPath;
    private readonly string _value;
    private readonly double _threshold;

    public TrigramFuzzySpecification(string propertyPath, string value, double threshold)
    {
        _propertyPath = propertyPath;
        _value = value;
        _threshold = threshold;
    }

    public Expression<Func<TEntity, bool>> ToExpression()
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");

        // Resolve nested property path: e.Name, e.Site.Postcode, etc.
        Expression property = ResolvePropertyPath(parameter, _propertyPath);

        // Build: EF.Functions.TrigramsWordSimilarity(value, property) >= threshold
        Expression efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        MethodInfo? similarityMethod =
            typeof(NpgsqlTrigramsDbFunctionsExtensions)
                .GetMethod(
                    nameof(NpgsqlTrigramsDbFunctionsExtensions.TrigramsWordSimilarity),
                    [typeof(DbFunctions), typeof(string), typeof(string)]
                );

        Expression similarityCall =
            Expression.Call(
                similarityMethod!,
                efFunctions,
                Expression.Constant(_value),
                property);

        Expression thresholdExpr = Expression.Constant(_threshold);
        Expression comparison = Expression.GreaterThanOrEqual(similarityCall, thresholdExpr);

        return Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
    }

    public bool IsSatisfiedBy(TEntity input)
    {
        Func<TEntity, bool> compiled = ToExpression().Compile();
        return compiled(input);
    }

    private static Expression ResolvePropertyPath(Expression root, string path)
    {
        Expression current = root;

        foreach (string segment in path.Split('.'))
        {
            current = Expression.PropertyOrField(current, segment);
        }

        return current;
    }
}
