using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Datasets.Application.UseCases.Request;

public sealed class DownloadDatasetsRequestTests
{
    [Fact]
    public void Constructor_ValidFilename_SetsFilenameProperty()
    {
        // arrange
        const string filename = "dataset.csv";

        // act
        DownloadDatasetsRequest request = new(filename);

        // assert
        Assert.Equal(filename, request.Filename);
    }

    [Fact]
    public void Constructor_NullFilename_ThrowsArgumentNullException()
    {
        // arrange
        string? filename = null;

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new DownloadDatasetsRequest(filename!));

        // assert
        Assert.Equal("filename", exception.ParamName);
    }

    [Fact]
    public void FilenameProperty_IsReadOnly()
    {
        // arrange
        DownloadDatasetsRequest request = new("dataset.csv");

        // act
        string value = request.Filename;

        // assert
        Assert.Equal("dataset.csv", value);
    }

    [Fact]
    public void Type_ImplementsCorrectUseCaseRequestInterface()
    {
        // arrange
        DownloadDatasetsRequest request = new("dataset.csv");

        // act
        bool implementsInterface =
            request is IUseCaseRequest<UseCaseResponse<DownloadDatasetsResponse>>;

        // assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public void GenericTypeParameter_IsUseCaseResponseOfDownloadDatasetsResponse()
    {
        // arrange
        Type requestType = typeof(DownloadDatasetsRequest);

        // act
        Type interfaceType =
            typeof(IUseCaseRequest<UseCaseResponse<DownloadDatasetsResponse>>);

        bool matches =
            interfaceType.IsAssignableFrom(requestType);

        // assert
        Assert.True(matches);
    }
}

