using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentTests
{
    [Fact]
    public void Urn_ShouldThrow_WhenInvalid()
    {
        Assert.Throws<EstablishmentException>(() => new EstablishmentUrn(new UniqueReferenceNumber("ABC")));
    }

    [Fact]
    public void ShouldSetUrn_WhenInitialized()
    {
        EstablishmentUrn urn = new EstablishmentUrn(new UniqueReferenceNumber("123456"));

        Establishment establishment = new Establishment
        {
            Urn = urn
        };

        Assert.Equal(urn, establishment.Urn);
    }

    [Fact]
    public void ShouldPopulateAllFields()
    {
        Establishment establishment = new Establishment
        {
            Urn = new EstablishmentUrn(new UniqueReferenceNumber("123456")),
            Ukprn = new Ukprn("10000123"),
            Uprn = new EstablishmentUprn("20000234"),
            Name = new EstablishmentName("Test School"),
            Number = new EstablishmentNumber("123"),
            Address = new Address("Street", "Town", "County", "AB1 2CD"),
            Status = new EstablishmentStatus("Open"),
            Type = new EstablishmentType("Academy"),
            Phase = new PhaseOfEducation("Primary"),
            OpenDate = new EstablishmentOpenDate(DateTime.UtcNow),
            ReasonEstablishmentOpened = new EstablishmentOpenReason("New school"),
            CloseDate = null,
            ReasonEstablishmentClosed = null
        };

        Assert.Equal("Test School", establishment.Name.Value);
    }
}
