using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentTestBuilder
{
    private static long _nextId = 1;

    public static Establishment Create(
        string urn,
        string name,
        EstablishmentType type)
    {
        return new Establishment
        {
            EstablishmentId = _nextId++,
            Urn = urn,
            Name = name,
            EstablishmentType = type
        };
    }
}
