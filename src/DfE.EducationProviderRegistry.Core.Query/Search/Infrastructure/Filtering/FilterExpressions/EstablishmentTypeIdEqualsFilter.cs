using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class EstablishmentTypeIdEqualsFilter<TProjection>
    : ISearchFilter<TProjection>
    where TProjection : class
{
    
    public ISpecification<TProjection> CreateSpecification(
        SearchFilterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PropertyEqualsAnySpecification<TProjection>(
            propertyName: "EstablishmentTypeId",
            values: request.FilterValues);
    }
}
