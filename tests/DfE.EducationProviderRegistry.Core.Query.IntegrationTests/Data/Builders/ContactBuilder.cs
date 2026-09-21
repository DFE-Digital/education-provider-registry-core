using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Builders;

public sealed class ContactBuilder
{
    private readonly Contact _contact;

    public ContactBuilder(Contact contact)
    {
        _contact = contact;
    }

    public ContactBuilder WithWebsite(string value)
    {
        _contact.Website = value;
        return this;
    }

    public ContactBuilder WithTelephoneNumber(string value)
    {
        _contact.TelephoneNumber = value;
        return this;
    }
}
