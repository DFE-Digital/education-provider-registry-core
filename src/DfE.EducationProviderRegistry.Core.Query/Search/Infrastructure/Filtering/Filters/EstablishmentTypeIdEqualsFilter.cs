using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class EstablishmentTypeFilter : ISearchFilter<Establishment>
{
    public ISpecification<Establishment> CreateSpecification(
        SearchFilterRequest request)
    {
        IReadOnlyCollection<long> values =
        [
            .. request.FilterValues
                .Where((filterValue) => filterValue != null)
                .Select((value) => Convert.ToInt64(value))
        ];

        return new PropertyEqualsAnyValuesSpecification<Establishment, long>(
            x => x.EstablishmentTypeId,
            values);
    }
}
