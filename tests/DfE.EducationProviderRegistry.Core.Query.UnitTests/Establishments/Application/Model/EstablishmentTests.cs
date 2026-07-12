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
    public void ShouldSetUrn_WhenInitialized()
    {
        EstablishmentUrnModel urn = new(new UniqueReferenceNumber("123456"));

        EstablishmentDetailsModel establishment = new()
        {
            Urn = urn
        };

        Assert.Equal(urn, establishment.Urn);
    }

    [Fact]
    public void ShouldPopulateAllFields()
    {
        EstablishmentDetailsModel establishment = new()
        {
            Urn = new EstablishmentUrnModel(new UniqueReferenceNumber("123456")),
            Name = new EstablishmentNameModel("Test School"),
            Number = new EstablishmentNumberModel("123"),
            Status = new EstablishmentStatusModel("Open"),
            Type = new EstablishmentTypeModel("Academy"),
            Phase = new PhaseOfEducationModel("Primary"),
        };

        Assert.Equal("Test School", establishment.Name.Value);
    }
}
