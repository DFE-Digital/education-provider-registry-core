using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Datasets.Application.UseCases.Request;

public sealed class DownloadDatasetsRequestTests
{
    [Fact]
    public void Constructor_ValidArguments_SetsPropertiesCorrectly()
    {
        // arrange
        const string datasetName = "EducationalEstablishments";
        string[] fileNameKeys = ["establishment-csv", "contacts-csv"];

        // act
        DownloadDatasetsRequest request = new(datasetName, fileNameKeys);

        // assert
        Assert.Equal(datasetName, request.DatasetName);
        Assert.Equal(fileNameKeys, request.FileNameKeys);
    }

    [Fact]
    public void Constructor_NullDatasetName_ThrowsArgumentNullException()
    {
        // arrange
        string? datasetName = null;
        string[] fileNameKeys = ["establishment-csv"];

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new DownloadDatasetsRequest(datasetName!, fileNameKeys));

        // assert
        Assert.Equal("datasetName", exception.ParamName);
    }

    [Fact]
    public void Constructor_NullFileNameKeys_ThrowsArgumentNullException()
    {
        // arrange
        const string datasetName = "EducationalEstablishments";
        string[]? fileNameKeys = null;

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new DownloadDatasetsRequest(datasetName, fileNameKeys!));

        // assert
        Assert.Equal("fileNameKeys", exception.ParamName);
    }

    [Fact]
    public void Properties_AreReadOnly()
    {
        // arrange
        DownloadDatasetsRequest request = new("EducationalEstablishments", ["establishment-csv"]);

        // act & assert
        Assert.Equal("EducationalEstablishments", request.DatasetName);
        Assert.Single(request.FileNameKeys);
    }

    [Fact]
    public void Type_ImplementsCorrectUseCaseRequestInterface()
    {
        // arrange
        DownloadDatasetsRequest request = new("EducationalEstablishments", []);

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
