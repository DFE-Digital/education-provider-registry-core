using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Datasets.Application.UseCases.TestDoubles;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Datasets.Application.UseCases;

public sealed class DownloadDatasetsUseCaseTests
{
    [Fact]
    public async Task HandleRequest_ValidRequest_InvokesRepositoryCorrectly()
    {
        // arrange
        const string filename = "dataset.csv";
        Dataset expected = new(filename, "text/csv", "csv", 3, [1, 2, 3]);

        DatasetDownloadRepositoryTestDouble.Capture capture = new();

        Mock<IDatasetDownloadRepository> repository =
            DatasetDownloadRepositoryTestDouble.ForCapturing(expected, capture);

        Mock<ILogger<DownloadDatasetsUseCase>> logger =
            MockTestDouble.Default<ILogger<DownloadDatasetsUseCase>>(MockBehavior.Loose);

        DownloadDatasetsUseCase sut = new(repository.Object, logger.Object);
        DownloadDatasetsRequest request = new(filename);

        // act
        UseCaseResponse<DownloadDatasetsResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        Assert.Equal(filename, capture.Filename);

        repository.Verify(datasetDownloadRepository =>
            datasetDownloadRepository.GetDatasetByName(
                filename,
                It.IsAny<CancellationToken>()),
            Times.Once);

        logger.VerifyNoErrors();

        // assert
        Assert.NotNull(response.Model);
        Assert.Equal(expected, response.Model.DownloadedDataset);
        Assert.True(response.SuccessfulRequest);
    }

    [Fact]
    public async Task HandleRequest_NullRequest_ThrowsAndReturnsFailure()
    {
        // arrange
        Mock<IDatasetDownloadRepository> repository =
            DatasetDownloadRepositoryTestDouble.For(null);

        Mock<ILogger<DownloadDatasetsUseCase>> logger =
            MockTestDouble.Default<ILogger<DownloadDatasetsUseCase>>(MockBehavior.Loose);

        DownloadDatasetsUseCase sut = new(repository.Object, logger.Object);

        // act
        UseCaseResponse<DownloadDatasetsResponse> response =
            await sut.HandleRequestAsync(null!, TestContext.Current.CancellationToken);

        // verify
        repository.Verify(datasetDownloadRepository =>
            datasetDownloadRepository.GetDatasetByName(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        logger.VerifyErrorContains("unexpected error");
        logger.VerifyErrorContains("An unexpected error occurred while processing the download request.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal(
            "An unexpected error occurred while processing the download request.",
            response.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequest_RepositoryThrowsOperationCanceledException_ReturnsCancelledError()
    {
        // arrange
        Mock<IDatasetDownloadRepository> repository =
            DatasetDownloadRepositoryTestDouble.Throwing(new OperationCanceledException());

        Mock<ILogger<DownloadDatasetsUseCase>> logger =
            MockTestDouble.Default<ILogger<DownloadDatasetsUseCase>>(MockBehavior.Loose);

        DownloadDatasetsUseCase sut =
            new(repository.Object, logger.Object);

        DownloadDatasetsRequest request = new("dataset.csv");

        // act
        UseCaseResponse<DownloadDatasetsResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        logger.VerifyWarningContains("DownloadDatasetsUseCase execution cancelled");
        logger.VerifyWarningContains("The download request was cancelled by the caller.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal("The download request was cancelled by the caller.", response.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequest_RepositoryThrowsSearchException_ReturnsUnexpectedError()
    {
        // arrange
        Mock<IDatasetDownloadRepository> repository =
            DatasetDownloadRepositoryTestDouble.Throwing(new SearchException("boom"));

        Mock<ILogger<DownloadDatasetsUseCase>> logger =
            MockTestDouble.Default<ILogger<DownloadDatasetsUseCase>>(MockBehavior.Loose);

        DownloadDatasetsUseCase sut =
            new(repository.Object, logger.Object);

        DownloadDatasetsRequest request = new("dataset.csv");

        // act
        UseCaseResponse<DownloadDatasetsResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        logger.VerifyErrorContains("DownloadDatasetsUseCase unexpected error");
        logger.VerifyErrorContains("An unexpected error occurred while processing the download request.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal(
            "An unexpected error occurred while processing the download request.",
            response.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequest_RepositoryThrowsUnexpectedException_ReturnsUnexpectedError()
    {
        // arrange
        Mock<IDatasetDownloadRepository> repository =
            DatasetDownloadRepositoryTestDouble.Throwing(new ApplicationException("boom"));

        Mock<ILogger<DownloadDatasetsUseCase>> logger =
            MockTestDouble.Default<ILogger<DownloadDatasetsUseCase>>(MockBehavior.Loose);

        DownloadDatasetsUseCase sut =
            new(repository.Object, logger.Object);

        DownloadDatasetsRequest request = new("dataset.csv");

        // act
        UseCaseResponse<DownloadDatasetsResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        logger.VerifyErrorContains("unexpected error");
        logger.VerifyErrorContains("An unexpected error occurred while processing the download request.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal(
            "An unexpected error occurred while processing the download request.",
            response.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequest_NoDatasetFound_ReturnsSuccessWithNullModel()
    {
        // arrange
        Mock<IDatasetDownloadRepository> repository =
            DatasetDownloadRepositoryTestDouble.For(null);

        Mock<ILogger<DownloadDatasetsUseCase>> logger =
            MockTestDouble.Default<ILogger<DownloadDatasetsUseCase>>(MockBehavior.Loose);

        DownloadDatasetsUseCase sut =
            new(repository.Object, logger.Object);

        DownloadDatasetsRequest request = new("missing.csv");

        // act
        UseCaseResponse<DownloadDatasetsResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        logger.VerifyNoErrors();

        // assert
        Assert.NotNull(response.Model);
        Assert.Null(response.Model.DownloadedDataset);
        Assert.True(response.SuccessfulRequest);
    }
}
