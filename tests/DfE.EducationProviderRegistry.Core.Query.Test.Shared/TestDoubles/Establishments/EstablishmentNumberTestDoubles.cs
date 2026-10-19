using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Establishments;

public static class EstablishmentNumberTestDoubles
{
    public static EstablishmentNumber Create() => new(
        new Random().Next(
            minValue: 1000,
            maxValue: 9999));
}
