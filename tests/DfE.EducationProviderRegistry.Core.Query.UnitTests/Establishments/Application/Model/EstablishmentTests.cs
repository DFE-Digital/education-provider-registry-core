using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentTests
{
    [Fact]
    public void Urn_ShouldThrow_WhenInvalid()
    {
        Assert.Throws<ArgumentException>(() => new EstablishmentUrnModel(new UniqueReferenceNumber("ABC")));
    }

    [Fact]
    public void ShouldPopulateAllFields()
    {
        EstablishmentDetailsModel establishment = new()
        {
            Name = new EstablishmentNameModel("Test School"),
            Identifiers = new EstablishmentIdentifiersModel(
                urn: new EstablishmentUrnModel(new UniqueReferenceNumber("123456")),
                ukprn: new Ukprn("12345678"),
                dfeNumber: new DfeNumber("123/1234")),
            Status = new EstablishmentStatusModel(EstablishmentStatus.Open, new(2026, 3, 1)),
            Type = new EstablishmentTypeModel("Academy"),
            Phase = new PhaseOfEducationModel("Primary"),
        };

        Assert.Equal("Test School", establishment.Name.Value);
        Assert.Equal("123456", establishment.Identifiers.Urn);
        Assert.Equal("123/1234", establishment.Identifiers.DfENumber);
        Assert.Equal(EstablishmentStatus.Open, establishment.Status.Status);
        Assert.Equal(new DateOnly(2026, 3, 1), establishment.Status.EffectiveDate);
    }
}
