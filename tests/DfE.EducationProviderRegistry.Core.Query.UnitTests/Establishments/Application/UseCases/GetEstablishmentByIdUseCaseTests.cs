using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.UseCases;

public sealed class GetEstablishmentByIdUseCaseTests
{
    private static readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<ILogger<GetEstablishmentByIdUseCase>> _loggerMock;

    private static GetEstablishmentByIdUseCase CreateSut(
        Mock<ILogger<GetEstablishmentByIdUseCase>> loggerMock,
        Mock<IEstablishmentsRepository> repoMock) => new(loggerMock.Object, repoMock.Object);

    public GetEstablishmentByIdUseCaseTests()
    {
        _loggerMock = MockTestDouble.Default<ILogger<GetEstablishmentByIdUseCase>>(MockBehavior.Loose);
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

        GetEstablishmentByIdUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByIdRequest request = new(establishment.Identifiers.Urn);

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

        GetEstablishmentByIdUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByIdRequest request = new("12345");

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
        GetEstablishmentByIdUseCase sut = CreateSut(_loggerMock, new Mock<IEstablishmentsRepository>());
        GetEstablishmentByIdRequest request = new("INVALID_URN");

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

        GetEstablishmentByIdUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByIdRequest request = new("12345");

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

        GetEstablishmentByIdUseCase sut = CreateSut(_loggerMock, repoMock);

        GetEstablishmentByIdRequest request = new("12345");

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
