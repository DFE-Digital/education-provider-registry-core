using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

internal static class SpecificationTestDoubles
{
    internal static ISpecification<T> Create<T>(
        Expression<Func<T, bool>>? expression = null)
    {
        return new TestSpecification<T>(expression ?? ((t) => true));
    }

    private sealed class TestSpecification<T> : ISpecification<T>
    {
        private readonly Expression<Func<T, bool>> _expression;

        public TestSpecification(Expression<Func<T, bool>> expression)
        {
            ArgumentNullException.ThrowIfNull(expression);
            _expression = expression;
        }

        public bool IsSatisfiedBy(T input) => _expression.Compile().Invoke(input);

        public Expression<Func<T, bool>> ToExpression()
        {
            return _expression;
        }
    }
}
