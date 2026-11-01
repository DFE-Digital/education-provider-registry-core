using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

public static class FacetOptionsTestDouble
{
    public static FacetOptions Stub(
        string facet,
        string valueProperty,
        string? labelProperty = null)
    {
        return new FacetOptions
        {
            Definitions = new Dictionary<string, FacetDefinition>
            {
                {
                    facet,
                    new FacetDefinition
                    {
                        ValueProperty = valueProperty,
                        LabelProperty = labelProperty,
                    }
                }
            }
        };
    }

    public static FacetOptions Empty()
    {
        return new FacetOptions();
    }
}

