using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;

internal static class SearchRequestFactory
{
    internal static SearchRequest BuildSearchRequest(IEnumerable<(string key, string value)> searchTerms)
    {
        SearchRequestBuilder requestBuilder = SearchRequestBuilder.Create();

        foreach ((string key, string value) termTuple in searchTerms)
        {
            requestBuilder.WithSearchTerm(termTuple);
        }

        return requestBuilder.Build();
    }
}
