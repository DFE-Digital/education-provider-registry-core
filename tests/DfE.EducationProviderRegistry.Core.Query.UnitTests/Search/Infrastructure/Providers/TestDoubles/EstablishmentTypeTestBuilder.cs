using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentTypeTestBuilder
{
    private static readonly Dictionary<long, EstablishmentFamily> _families = [];
    private static readonly Dictionary<long, EstablishmentType> _types = [];

    public static void Reset()
    {
        _families.Clear();
        _types.Clear();
    }

    public static EstablishmentType Create(
    long establishmentTypeId,
    long establishmentFamilyId,
    string code,
    string name,
    bool isSchool,
    bool isGroup,
    bool isEarlyYears,
    bool isFurtherEducation)
    {
        // Reuse EstablishmentFamily
        if (!_families.TryGetValue(establishmentFamilyId, out EstablishmentFamily? family))
        {
            family = new EstablishmentFamily
            {
                EstablishmentFamilyId = establishmentFamilyId,
                Name = "Test Family",
                Code = "FAM" + establishmentFamilyId
            };

            _families[establishmentFamilyId] = family;
        }

        // Reuse EstablishmentType
        if (_types.TryGetValue(establishmentTypeId, out EstablishmentType? existingType))
        {
            return existingType;
        }

        EstablishmentType type = new()
        {
            EstablishmentTypeId = establishmentTypeId,
            EstablishmentFamilyId = establishmentFamilyId,
            Code = code,
            Name = name,
            IsSchool = isSchool,
            IsGroup = isGroup,
            IsEarlyYears = isEarlyYears,
            IsFurtherEducation = isFurtherEducation,
            EstablishmentFamily = family,
            Establishment = []
        };

        _types[establishmentTypeId] = type;
        return type;
    }
}
