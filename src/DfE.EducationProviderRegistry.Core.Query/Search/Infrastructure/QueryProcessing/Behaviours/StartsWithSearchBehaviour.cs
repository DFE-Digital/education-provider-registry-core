using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

internal sealed class StartsWithSearchBehaviour<TEntity> : ISearchBehaviour<TEntity>
{

    public ISpecification<TEntity> Build(string propertyPath, string value)
        => new StartsWithSpecification<TEntity>(propertyPath, value);
}
