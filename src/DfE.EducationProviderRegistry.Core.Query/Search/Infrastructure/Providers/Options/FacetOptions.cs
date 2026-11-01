using System.ComponentModel.DataAnnotations;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;

public sealed class FacetOptions
{
    public IDictionary<string, FacetDefinition> Definitions { get; set; } = new Dictionary<string, FacetDefinition>();
}

public sealed class FacetDefinition
{
    public string? LabelProperty { get; set; }

#nullable disable
    [Required(AllowEmptyStrings = false)]
    public string ValueProperty { get; set; }
#nullable enable
}
