using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Mappers;

internal sealed class SearchRequestTermToSearchRequestMapper :
    IMapper<IReadOnlyCollection<SearchTermRequest>, IReadOnlyCollection<SearchTerm>>
{
    public IReadOnlyCollection<SearchTerm> Map(IReadOnlyCollection<SearchTermRequest> input)
    {
        return input
            .Where(searchTerm =>
                searchTerm is not null &&
                !string.IsNullOrWhiteSpace(searchTerm.Key) &&
                !string.IsNullOrWhiteSpace(searchTerm.Value))
            .Select(searchTerm => new SearchTerm(searchTerm.Key, searchTerm.Value))
            .ToList();
    }
}
