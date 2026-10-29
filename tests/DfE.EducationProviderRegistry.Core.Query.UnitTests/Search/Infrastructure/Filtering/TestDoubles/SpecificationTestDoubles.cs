using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

internal static class SpecificationTestDoubles
{
    public static ISpecification<T> True<T>() => new Fake<T>((t) => true);
    public static ISpecification<T> False<T>() => new Fake<T>((t) => false);

    internal static ISpecification<T> Create<T>(Expression<Func<T, bool>>? expression = null)
    {
        return new Fake<T>(expression ?? ((t) => true));
    }

    private sealed class Fake<T> : ISpecification<T>
    {
        private readonly Expression<Func<T, bool>> _expression;

        public Fake(Expression<Func<T, bool>> expression)
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
