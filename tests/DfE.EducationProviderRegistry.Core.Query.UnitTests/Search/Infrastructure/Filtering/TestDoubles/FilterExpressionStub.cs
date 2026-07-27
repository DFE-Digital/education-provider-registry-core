using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal sealed class FilterExpressionStub<TProjection> : ISearchFilterExpression<TProjection>
    where TProjection : class
{
    private readonly Expression<Func<TProjection, bool>> _expression;

    public FilterExpressionStub(Expression<Func<TProjection, bool>> expression)
    {
        _expression = expression;
    }

    public Expression<Func<TProjection, bool>> ToExpression(SearchFilterRequest request)
    {
        return _expression;
    }
}
