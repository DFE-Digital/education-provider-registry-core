using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;

internal static class SearchRequestFactory
{
    internal static SearchRequest BuildSearchRequest(
        IEnumerable<(string key, string value)> searchTerms,
        IEnumerable<FilterRequest> filters)
    {
        SearchRequestBuilder requestBuilder = SearchRequestBuilder.Create();

        foreach ((string key, string value) termTuple in searchTerms)
        {
            requestBuilder.WithSearchTerm(termTuple);
        }

        requestBuilder.WithFilterRequests(filters);

        return requestBuilder.Build();
    }
}
