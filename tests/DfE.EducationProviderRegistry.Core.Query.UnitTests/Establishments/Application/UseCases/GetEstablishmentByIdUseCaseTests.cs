using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentByUrn;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.UseCases;

public sealed class GetEstablishmentByIdUseCaseTests
{
    private static readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<ILogger<GetEstablishmentByUrnUseCase>> _loggerMock;

    private static GetEstablishmentByUrnUseCase CreateSut(
        Mock<ILogger<GetEstablishmentByUrnUseCase>> loggerMock,
        Mock<IEstablishmentsRepository> repoMock) => new(loggerMock.Object, repoMock.Object);

    public GetEstablishmentByIdUseCaseTests()
    {
        _loggerMock = MockTestDouble.Default<ILogger<GetEstablishmentByUrnUseCase>>(MockBehavior.Loose);
    }

    [Fact]
    public async Task HandleRequestAsync_ReturnsMappedEstablishmentDetails()
    {
        // Arrange
        EstablishmentDetailsModel establishment =
            new EstablishmentDetailsModelCollectionBuilder()
                .WithCount(1)
                .Build()
                .Single();

        Mock<IEstablishmentsRepository> repoMock =
            MockTestDouble.For<IEstablishmentsRepository, EstablishmentDetailsModel?>(
                (repo) => repo.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token),
                establishment);

        GetEstablishmentByUrnUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByUrnRequest request = new(establishment.Identifiers.Urn);

        // Act
        UseCaseResponse<EstablishmentDetailsReadModel> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.True(result.SuccessfulRequest);
        Assert.NotNull(result.Model);
        Assert.NotNull(result.Model.Establishment);
        Assert.Equal(establishment.Identifiers.Urn, result.Model.Establishment.Identifiers.Urn);

        repoMock.Verify(
            r => r.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token),
            Times.Once);

        _loggerMock.VerifyNoErrors();
    }

    [Fact]
    public async Task HandleRequestAsync_WhenRepositoryReturnsNull_ReturnsSuccessWithNullModel()
    {
        // Arrange
        Mock<IEstablishmentsRepository> repoMock =
            MockTestDouble.For<IEstablishmentsRepository, EstablishmentDetailsModel?>(
                (repo) => repo.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token),
                null!);

        GetEstablishmentByUrnUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByUrnRequest request = new("12345");

        // Act
        UseCaseResponse<EstablishmentDetailsReadModel> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.True(result.SuccessfulRequest);
        Assert.Null(result.Model.Establishment);

        repoMock.Verify(
            r => r.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token),
            Times.Once);

        _loggerMock.VerifyNoErrors();
    }

    [Fact]
    public async Task HandleRequestAsync_WithInvalidUrn_ReturnsFailure()
    {
        // Arrange
        GetEstablishmentByUrnUseCase sut = CreateSut(_loggerMock, new Mock<IEstablishmentsRepository>());
        GetEstablishmentByUrnRequest request = new("INVALID_URN");

        // Act
        UseCaseResponse<EstablishmentDetailsReadModel> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Equal("An unexpected error occurred while processing the request.", result.ErrorMessage);

        _loggerMock.VerifyErrorContains("unexpected error");
    }

    [Fact]
    public async Task HandleRequestAsync_ThrowsOperationCanceledException_ReturnsCorrectErrorResponse()
    {
        // Arrange
        Mock<IEstablishmentsRepository> repoMock =
            MockTestDouble.ThrowsExceptionFor<
                IEstablishmentsRepository,
                EstablishmentDetailsModel?,
                OperationCanceledException>(
                    (repo) => repo.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token));

        GetEstablishmentByUrnUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByUrnRequest request = new("12345");

        // Act
        UseCaseResponse<EstablishmentDetailsReadModel> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Equal("The request was cancelled by the caller.", result.ErrorMessage);

        repoMock.Verify(
            r => r.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token),
            Times.Once);

        _loggerMock.VerifyErrorContains("execution was cancelled by the caller");
    }

    [Fact]
    public async Task HandleRequestAsync_ThrowsException_ReturnsCorrectErrorResponse()
    {
        // Arrange
        Mock<IEstablishmentsRepository> repoMock =
            MockTestDouble.ThrowsExceptionFor<
                IEstablishmentsRepository,
                EstablishmentDetailsModel?,
                Exception>(
                    (repo) => repo.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token));

        GetEstablishmentByUrnUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByUrnRequest request = new("12345");

        // Act
        UseCaseResponse<EstablishmentDetailsReadModel> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Equal("An unexpected error occurred while processing the request.", result.ErrorMessage);

        repoMock.Verify(
            r => r.GetEstablishmentById(It.IsAny<EstablishmentUrnModel>(), _token),
            Times.Once);

        _loggerMock.VerifyErrorContains("unexpected error");
    }
}
