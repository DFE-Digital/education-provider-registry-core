using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

internal sealed class FuzzySearchBehaviour<TEntity> : ISearchBehaviour<TEntity>
{
    private const double DefaultSimilarityThreshold = 0.8;

    public string Name => "fuzzy";

    public ISpecification<TEntity> Build(string propertyPath, string value)
        => new TrigramFuzzySpecification<TEntity>(propertyPath, value, DefaultSimilarityThreshold);
}
