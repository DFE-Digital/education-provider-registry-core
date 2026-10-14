using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Mappers;

internal sealed class SearchRequestTermToSearchRequestMapper : IMapper<IReadOnlyCollection<SearchTermRequest>, IReadOnlyCollection<SearchTerm>>
{
    public IReadOnlyCollection<SearchTerm> Map(IReadOnlyCollection<SearchTermRequest> input)
    {
        if (input is null || input.Count == 0)
        {
            return [];
        }

        List<SearchTerm> terms = [];

        foreach (SearchTermRequest request in input)
        {
            if (request is null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(request.Key))
            {
                continue;
            }

            terms.Add(
                new SearchTerm(key: request.Key, value: request.Value));
        }

        return terms;
    }
}
