using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.QueryEngine.DataTransferObjects;

public sealed class GroupDownloadDtoTests
{
    [Fact]
    public void GroupUniqueIdentifierUID_SetValue_ReturnsValue()
    {
        // Arrange
        GroupDownloadDto sut = new()
        {
            // Act
            GroupUniqueIdentifierUID = "500001"
        };

        // Assert
        Assert.Equal("500001", sut.GroupUniqueIdentifierUID);
    }

    [Fact]
    public void GroupId_SetValue_ReturnsValue()
    {
        // Arrange
        GroupDownloadDto sut = new()
        {
            // Act
            GroupId = "G01"
        };

        // Assert
        Assert.Equal("G01", sut.GroupId);
    }

    [Fact]
    public void GroupName_SetValue_ReturnsValue()
    {
        // Arrange
        GroupDownloadDto sut = new()
        {
            // Act
            GroupName = "Trust Alpha"
        };

        // Assert
        Assert.Equal("Trust Alpha", sut.GroupName);
    }

    [Fact]
    public void GroupType_SetValue_ReturnsValue()
    {
        // Arrange
        GroupDownloadDto sut = new()
        {
            // Act
            GroupType = "Multi-Academy Trust"
        };

        // Assert
        Assert.Equal("Multi-Academy Trust", sut.GroupType);
    }

    [Fact]
    public void NewInstance_PropertiesAreNull()
    {
        // Act
        GroupDownloadDto sut = new();

        // Assert
        Assert.Null(sut.GroupUniqueIdentifierUID);
        Assert.Null(sut.GroupId);
        Assert.Null(sut.GroupName);
        Assert.Null(sut.GroupType);
    }
}
