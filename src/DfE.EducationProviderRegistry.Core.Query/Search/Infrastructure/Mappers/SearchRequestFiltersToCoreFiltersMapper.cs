using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

internal sealed class SearchRequestFiltersToCoreFiltersMapper :
    IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>>
{
    /// <summary>
    /// Converts a collection of <see cref="FilterRequest"/> into a read-only collection of
    /// <see cref="SearchFilterRequest"/> suitable for the core search engine.
    /// </summary>
    /// <param name="input">The incoming filter requests.</param>
    /// <returns>
    /// A read-only collection of mapped <see cref="SearchFilterRequest"/> objects.
    /// Returns an empty collection when no valid filters exist.
    /// </returns>
    public ReadOnlyCollection<SearchFilterRequest> Map(ReadOnlyCollection<FilterRequest> input)
    {
        if (input == null || input.Count == 0)
        {
            return new List<SearchFilterRequest>().AsReadOnly();
        }

        List<SearchFilterRequest> results = new(input.Count);

        foreach (FilterRequest filter in input)
        {
            if (filter.FilterValues == null || filter.FilterValues.Count == 0)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(filter.FilterName))
            {
                continue;
            }

            object[] copiedValues = new object[filter.FilterValues.Count];
            filter.FilterValues.CopyTo(copiedValues, 0);

            SearchFilterRequest mapped =
                new(
                    filterKey: filter.FilterName,
                    filterValues: copiedValues);

            results.Add(mapped);
        }

        return results.AsReadOnly();
    }
}
