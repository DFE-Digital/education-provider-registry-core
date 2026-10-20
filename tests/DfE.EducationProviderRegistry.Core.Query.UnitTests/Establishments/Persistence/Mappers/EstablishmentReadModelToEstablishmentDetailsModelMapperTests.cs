using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Establishments;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers;

public sealed class EstablishmentReadModelToEstablishmentDetailsModelMapperTests
{
    private readonly EstablishmentReadModelToEstablishmentDetailsModelMapper _mapper = new();


    [Fact]
    public void Map_WithNullDto_ThrowsArgumentNullException()
    {
        // Arrange
        EstablishmentReadModelToEstablishmentDetailsModelMapper mapper = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_MapsUrn()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithUrn("123456")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Equal(
            establishment.Urn,
            result.Identifiers.Urn);
    }

    [Fact]
    public void Map_WithLocalAuthorityCodeAndEstablishmentNumber_MapsDfeNumber()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithLocalAuthorityCode(123)
                .WithEstablishmentNumber("4567")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Equal(
            "123/4567",
            result.Identifiers.DfENumber);
    }

    [Fact]
    public void Map_WithoutLocalAuthorityCode_DoesNotMapDfeNumber()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithLocalAuthorityCode(null)
                .WithEstablishmentNumber("4567")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.Identifiers.DfENumber);
    }

    [Fact]
    public void Map_WithoutEstablishmentNumber_DoesNotMapDfeNumber()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithLocalAuthorityCode(123)
                .WithEstablishmentNumber(null)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.Identifiers.DfENumber);
    }

    [Fact]
    public void Map_WithStatusCode_MapsStatus()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithStatusCode(EstablishmentStatus.Open)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.NotNull(result.Status);
        Assert.Equal(1, (int)result.Status.Status);
    }

    [Fact]
    public void Map_WithoutStatusCode_ReturnsNullStatus()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithStatusCode(null)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.Status);
    }

    [Fact]
    public void Map_WithoutGroupName_ReturnsNullGroup()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithGroupName(null)
                .WithGroupCode("TRST")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.Group);
    }

    [Fact]
    public void Map_WithoutGroupCode_ReturnsNullGroup()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithGroupName("Test Group")
                .WithGroupCode(null)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.Group);
    }

    [Fact]
    public void Map_WithAddressDetails_MapsAddress()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithSiteName("Main Site")
                .WithAddressLine1("1 High Street")
                .WithTown("Retford")
                .WithCounty("Nottinghamshire")
                .WithPostcode("DN22 1AA")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.NotNull(result.Address);
        Assert.Equal("Main Site", result.Address.Name);
        Assert.Equal("1 High Street", result.Address.AddressLine1);
        Assert.Equal("Retford", result.Address.Town);
        Assert.Equal("Nottinghamshire", result.Address.County);
        Assert.Equal("DN22 1AA", result.Address.Postcode);
    }

    [Fact]
    public void Map_WithoutAddressDetails_ReturnsNullAddress()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithSiteName(null)
                .WithAddressLine1(null)
                .WithTown(null)
                .WithCounty(null)
                .WithPostcode(null)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.Address);
    }

    [Fact]
    public void Map_WithLocalAuthorityCodeAndName_MapsLocalAuthority()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithLocalAuthorityCode(123)
                .WithLocalAuthorityName("Test Authority")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.NotNull(result.LocalAuthority);
        Assert.Equal("Test Authority", result.LocalAuthority.Name);
        Assert.Equal(123, result.LocalAuthority.Code);
    }

    [Fact]
    public void Map_WithoutLocalAuthorityCode_ReturnsNullLocalAuthority()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithLocalAuthorityCode(null)
                .WithLocalAuthorityName("Test Authority")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.LocalAuthority);
    }

    [Fact]
    public void Map_WithoutLowAge_ReturnsNullAgeRange()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithLowAge(null)
                .WithHighAge(11)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.AgeRange);
    }

    [Fact]
    public void Map_WithStatutoryAges_MapsAgeRange()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithLowAge(5)
                .WithHighAge(11)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Equal("5 to 11", result.AgeRange);
    }

    [Fact]
    public void Map_WithGovernors_MapsGovernors()
    {
        // Arrange
        IReadOnlyList<EstablishmentGovernorAggregate> governors =
            [.. EstablishmentGovernorAggregateTestDoubles.StubMany()];

        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .Build();

        EstablishmentReadModel input =
            new(establishment, governors);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Equal(governors.Count, result.Governors!.Count());

        // Assert
        Assert.Equal(2, result.Governors!.Count());

        Assert.Contains(
            result.Governors!,
            g => g.Identifier.Value == governors[0].GovernorId);

        Assert.Contains(
            result.Governors!,
            g => g.Identifier.Value == governors[1].GovernorId);
    }

    [Fact]
    public void Map_WithoutGovernors_ReturnsEmptyGovernorsCollection()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Empty(result.Governors!);
    }


    [Fact]
    public void Map_WithWebsiteAndTelephone_MapsContactDetails()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithWebsite("www.test.com")
                .WithTelephoneNumber("0123456789")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.NotNull(result.ContactDetails);
        Assert.Equal("www.test.com", result.ContactDetails.Website);
        Assert.Equal("0123456789", result.ContactDetails.TelephoneNumber);
    }

    [Fact]
    public void Map_WithoutWebsiteAndTelephone_ReturnsNullContactDetails()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithWebsite(null)
                .WithTelephoneNumber(null)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.ContactDetails);
    }


    [Fact]
    public void Map_WithOfstedDetails_MapsOfsted()
    {
        // Arrange
        DateOnly inspectionDate = new(2024, 1, 1);

        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithOfstedInspectionDate(inspectionDate)
                .WithOfstedReportUrl("https://ofsted.example/report")
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.NotNull(result.Ofsted);
        Assert.Equal(inspectionDate, result.Ofsted.InspectionDate);

        Assert.Equal(
            "https://ofsted.example/report",
            result.Ofsted.ReportUrl);
    }

    [Fact]
    public void Map_WithoutOfstedDetails_ReturnsNullOfsted()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithOfstedInspectionDate(null)
                .WithOfstedReportUrl(null)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.Null(result.Ofsted);
    }



    [Fact]
    public void Map_WithGroupNameAndCode_MapsGroup()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithGroupName("Test Group")
                .WithGroupUid(123L)
                .Build();

        EstablishmentReadModel input =
            new(establishment, []);

        // Act
        EstablishmentDetailsModel result =
            _mapper.Map(input);

        // Assert
        Assert.NotNull(result.Group);
        Assert.Equal("Test Group", result.Group.name);
        Assert.Equal("123", result.Group.uid);
    }
}
