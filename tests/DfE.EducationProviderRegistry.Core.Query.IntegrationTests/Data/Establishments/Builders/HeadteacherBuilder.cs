using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using static DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.EstablishmentBuilder;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Builders;

public sealed class HeadteacherBuilder
{
    private readonly RoleAssignment _roleAssignment;
    private readonly EstablishmentReferenceData _referenceData;

    public HeadteacherBuilder(
        RoleAssignment roleAssignment,
        EstablishmentReferenceData referenceData)
    {
        _roleAssignment = roleAssignment;
        _referenceData = referenceData;
    }

    public HeadteacherBuilder WithGivenName(string value)
    {
        _roleAssignment.Role.Person.GivenName = value;

        UpdateDisplayName();

        return this;
    }

    public HeadteacherBuilder WithFamilyName(string value)
    {
        _roleAssignment.Role.Person.FamilyName = value;

        UpdateDisplayName();

        return this;
    }

    public HeadteacherBuilder WithRoleType(string code)
    {
        _referenceData.HeadteacherRoleTypeCode = code;

        return this;
    }

    private void UpdateDisplayName()
    {
        Person person = _roleAssignment.Role.Person;

        person.DisplayName =
            $"{person.GivenName} {person.FamilyName}";
    }
}
