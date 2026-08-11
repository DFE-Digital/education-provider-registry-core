using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

internal sealed class SpecificationStub<TEntity> : ISpecification<TEntity>
        where TEntity : class
{
    private readonly Func<TEntity, bool> _delegateBehaviour;

    public SpecificationStub(Func<TEntity, bool> delegateBehaviour)
    {
        _delegateBehaviour = delegateBehaviour;
    }

    public bool IsSatisfiedBy(TEntity input) => _delegateBehaviour(input);

    public System.Linq.Expressions.Expression<Func<TEntity, bool>> ToExpression()
        => entity => _delegateBehaviour(entity);
}
