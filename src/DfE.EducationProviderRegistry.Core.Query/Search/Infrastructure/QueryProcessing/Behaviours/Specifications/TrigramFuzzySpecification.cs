using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

internal sealed class TrigramFuzzySpecification<TEntity>
    : PropertyPathSpecification<TEntity>
{
    private readonly string _value;
    private readonly double _threshold;

    public TrigramFuzzySpecification(string propertyPath, string value, double threshold)
        : base(propertyPath)
    {
        ArgumentNullException.ThrowIfNull(propertyPath);
        ArgumentNullException.ThrowIfNull(value);

        _value = value;
        _threshold = threshold;
    }

    private static MethodInfo GetSimilarityMethod()
    {
        return typeof(NpgsqlTrigramsDbFunctionsExtensions)
            .GetMethod(
                nameof(NpgsqlTrigramsDbFunctionsExtensions.TrigramsWordSimilarity),
                [typeof(DbFunctions), typeof(string), typeof(string)]
            )!;
    }

    protected override Expression BuildExpression(Expression access)
    {
        MemberExpression efFunctions =
            Expression.Property(null, typeof(EF), nameof(EF.Functions));

        MethodInfo similarityMethod = GetSimilarityMethod();

        Expression similarityCall =
            Expression.Call(
                similarityMethod,
                efFunctions,
                Expression.Constant(_value),
                access
            );

        Expression thresholdExpr = Expression.Constant(_threshold);

        Expression comparison =
            Expression.GreaterThanOrEqual(similarityCall, thresholdExpr);

        return comparison;
    }
}
