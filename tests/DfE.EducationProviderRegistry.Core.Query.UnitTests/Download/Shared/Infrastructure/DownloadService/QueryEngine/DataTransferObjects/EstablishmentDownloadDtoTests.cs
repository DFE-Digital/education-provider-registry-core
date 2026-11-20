using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.QueryEngine.DataTransferObjects;

public sealed class EstablishmentDownloadDtoTests
{
    [Fact]
    public void EstablishmentUniqueReferenceNumberUrn_SetValue_ReturnsValue()
    {
        // Arrange
        EstablishmentDownloadDto sut = new()
        {
            // Act
            EstablishmentUniqueReferenceNumberUrn = "100001"
        };

        // Assert
        Assert.Equal("100001", sut.EstablishmentUniqueReferenceNumberUrn);
    }

    [Fact]
    public void EstablishmentNumber_SetValue_ReturnsValue()
    {
        // Arrange
        EstablishmentDownloadDto sut = new()
        {
            // Act
            EstablishmentNumber = "001"
        };

        // Assert
        Assert.Equal("001", sut.EstablishmentNumber);
    }

    [Fact]
    public void EstablishmentName_SetValue_ReturnsValue()
    {
        // Arrange
        EstablishmentDownloadDto sut = new()
        {
            // Act
            EstablishmentName = "Open School"
        };

        // Assert
        Assert.Equal("Open School", sut.EstablishmentName);
    }

    [Fact]
    public void EstablishmentTypeName_SetValue_ReturnsValue()
    {
        // Arrange
        EstablishmentDownloadDto sut = new()
        {
            // Act
            EstablishmentTypeName = "Academy"
        };

        // Assert
        Assert.Equal("Academy", sut.EstablishmentTypeName);
    }

    [Fact]
    public void EducationPhaseName_SetValue_ReturnsValue()
    {
        // Arrange
        EstablishmentDownloadDto sut = new()
        {
            // Act
            EducationPhaseName = "Secondary"
        };

        // Assert
        Assert.Equal("Secondary", sut.EducationPhaseName);
    }

    [Fact]
    public void NewInstance_PropertiesAreNull()
    {
        // Act
        EstablishmentDownloadDto sut = new();

        // Assert
        Assert.Null(sut.EstablishmentUniqueReferenceNumberUrn);
        Assert.Null(sut.EstablishmentNumber);
        Assert.Null(sut.EstablishmentName);
        Assert.Null(sut.EstablishmentTypeName);
        Assert.Null(sut.EducationPhaseName);
    }
}
