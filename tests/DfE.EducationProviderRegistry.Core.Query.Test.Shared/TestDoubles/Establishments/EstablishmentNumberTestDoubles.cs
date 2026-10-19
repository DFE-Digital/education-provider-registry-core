using System.Security.Cryptography;
using Bogus.Bson;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Establishments;

public static class EstablishmentNumberTestDoubles
{
    public static EstablishmentNumber Create() => new(
            RandomNumberGenerator.GetInt32(fromInclusive: 1000, toExclusive: 10_000));
}
