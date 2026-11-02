using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Facets;

public static class FacetOptionsConfigurationStub
{
    public static Dictionary<string, string?> Stub()
    {
        return new Dictionary<string, string?>
        {
            [$"FacetOptions:Definitions:searchprovidertypename:ValueProperty"] =
                nameof(SearchAggregate.ProviderTypeName),

            [$"FacetOptions:Definitions:searchprovidertypename:LabelProperty"] =
                nameof(SearchAggregate.ProviderTypeName)
        };
    }
}
