using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Builders;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;

public sealed class EstablishmentBuilder
{
    private readonly Establishment _establishment;

    public EstablishmentBuilder()
    {
        ReferenceData = new();

        Person headteacher = new()
        {
            GivenName = "Test",
            FamilyName = "Headteacher",
            DisplayName = "Test Headteacher"
        };

        Role role = new()
        {
            Person = headteacher
        };

        _establishment = new Establishment
        {
            Name = "Test Establishment",
        };

        RoleAssignment roleAssignment = new()
        {
            Role = role
        };

        _establishment.RoleAssignment.Add(roleAssignment);
    }

    internal EstablishmentReferenceData ReferenceData { get; }

    public EstablishmentBuilder WithName(string value)
    {
        _establishment.Name = value;
        return this;
    }

    public EstablishmentBuilder WithUrn(string value)
    {
        _establishment.Urn = value;
        return this;
    }

    public EstablishmentBuilder WithUid(string value)
    {
        _establishment.Uid = value;
        return this;
    }

    public EstablishmentBuilder WithEstablishmentType(string code)
    {
        ReferenceData.EstablishmentTypeCode = code;
        return this;
    }

    public EstablishmentBuilder WithEstablishmentStatus(string code)
    {
        ReferenceData.EstablishmentStatusCode = code;
        return this;
    }

    public EstablishmentBuilder Headteacher(
        Action<HeadteacherBuilder> configure)
    {
        configure(
            new HeadteacherBuilder(
                _establishment.HeadteacherRoleAssignment!,
                ReferenceData));

        return this;
    }

    public EstablishmentBuilder Contact(
        Action<ContactBuilder> configure)
    {
        Contact contact =
            _establishment.Contact.SingleOrDefault()
            ?? CreateDefaultContact();

        configure(new ContactBuilder(contact));

        return this;
    }

    public Establishment Build() => _establishment;


    private Contact CreateDefaultContact()
    {
        Contact contact = new();

        _establishment.Contact.Add(contact);

        return contact;
    }

    // used for DB lookups to get the correct IDs for the establishment type, status, and headteacher role type
    public sealed class EstablishmentReferenceData
    {
        public string EstablishmentTypeCode { get; set; } = "PRI";
        public string EstablishmentStatusCode { get; set; } = "OPEN";
        public string HeadteacherRoleTypeCode { get; set; } = "HT";
    }
}
