using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Criteria;

internal static class SearchCriteriaConfigurationAdaptor
{
    public static Dictionary<string, string?> ToConfiguration(SearchCriteria options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Dictionary<string, string?> configuration = [];

        for (int index = 0; index < options.SearchFields.Count; index++)
        {
            configuration[
                $"{nameof(SearchCriteria)}:{nameof(SearchCriteria.SearchFields)}:{index}"] = options.SearchFields[index];
        }

        for (int index = 0; index < options.Facets.Count; index++)
        {
            configuration[
                $"{nameof(SearchCriteria)}:{nameof(SearchCriteria.Facets)}:{index}"] = options.Facets[index];
        }

        return configuration;
    }
}
