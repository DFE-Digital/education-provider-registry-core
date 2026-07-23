using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
public sealed class DummyProjection
{
    public string? Value { get; set; }
    public int EstablishmentTypeId { get; set; }
}
