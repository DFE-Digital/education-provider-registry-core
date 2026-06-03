using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.UseCases;

public class GetEstablishmentsUseCaseUnitTests
{
    private readonly CancellationToken _token = TestContext.Current.CancellationToken;

    private static GetEstablishmentsUseCase CreateSut(
        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock,
        Mock<IEstablishmentsRepository> repoMock)
    {
        return new GetEstablishmentsUseCase(loggerMock.Object, repoMock.Object);
    }

    [Fact]
    public async Task HandleRequestAsync_ReturnsMappedEstablishments()
    {
        // Arrange
        IReadOnlyCollection<Establishment> establishmentResults =
            new EstablishmentCollectionBuilder()
                .WithCount(2)
                .Build();

        Mock<IEstablishmentsRepository> repoMock =
            EstablishmentsRepositoryTestDouble
                .MockGetEstablishments(establishmentResults, _token);

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            ILoggerTestDouble.Default<GetEstablishmentsUseCase>();

        GetEstablishmentsUseCase sut = CreateSut(loggerMock, repoMock);

        GetEstablishmentsRequest request = new();

        // Act
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.True(result.SuccessfulRequest);

        Assert.Collection(
            result.Model!,
            establishment => Assert.Equal(establishmentResults.ElementAt(0).Identifier.Urn, establishment.Identifier.Urn),
            establishment => Assert.Equal(establishmentResults.ElementAt(1).Identifier.Urn, establishment.Identifier.Urn));

        repoMock.Verify(repository => repository.GetEstablishments(_token), Times.Once);
        loggerMock.VerifyNoErrors();
    }

    [Fact]
    public async Task HandleRequestAsync_WithNullRepositoryResponse_ReturnsEmptyResponse()
    {
        // Arrange
        Mock<IEstablishmentsRepository> repoMock =
            EstablishmentsRepositoryTestDouble
                .MockGetEstablishments(null!, _token);

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            ILoggerTestDouble.Default<GetEstablishmentsUseCase>();

        GetEstablishmentsUseCase sut = CreateSut(loggerMock, repoMock);

        GetEstablishmentsRequest request = new();

        // Act
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.True(result.SuccessfulRequest);
        Assert.Null(result.Model);

        repoMock.Verify(repository => repository.GetEstablishments(_token), Times.Once);
        loggerMock.VerifyNoErrors();
    }

    [Fact]
    public async Task HandleRequestAsync_ThrowsOperationCanceledException_ReturnsCorrectErrorResponse()
    {
        // Arrange
        Mock<IEstablishmentsRepository> repoMock =
            EstablishmentsRepositoryTestDouble
                .MockGetEstablishmentsThrowsOperationCanceled(_token);

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            ILoggerTestDouble.Default<GetEstablishmentsUseCase>();

        GetEstablishmentsUseCase sut = CreateSut(loggerMock, repoMock);

        GetEstablishmentsRequest request = new();

        // Act
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Equal("The request was cancelled by the caller.", result.ErrorMessage);

        repoMock.Verify(repository => repository.GetEstablishments(_token), Times.Once);
        loggerMock.VerifyErrorContains("execution was cancelled by the caller");
    }

    [Fact]
    public async Task HandleRequestAsync_ThrowsEstablishmentException_ReturnsCorrectErrorResponse()
    {
        // Arrange
        Mock<IEstablishmentsRepository> repoMock =
            EstablishmentsRepositoryTestDouble
                .MockGetEstablishmentsThrowsEstablishmentException(_token);

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            ILoggerTestDouble.Default<GetEstablishmentsUseCase>();

        GetEstablishmentsUseCase sut = CreateSut(loggerMock, repoMock);

        GetEstablishmentsRequest request = new();

        // Act
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Equal("Failed to retrieve establishment information from the repository.", result.ErrorMessage);

        repoMock.Verify(repository => repository.GetEstablishments(_token), Times.Once);
        loggerMock.VerifyErrorContains("encountered a domain-specific error");
    }

    [Fact]
    public async Task HandleRequestAsync_ThrowsException_ReturnsCorrectErrorResponse()
    {
        // Arrange
        Mock<IEstablishmentsRepository> repoMock =
            EstablishmentsRepositoryTestDouble
                .MockGetEstablishmentsThrowsException(_token);

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            ILoggerTestDouble.Default<GetEstablishmentsUseCase>();

        GetEstablishmentsUseCase sut = CreateSut(loggerMock, repoMock);

        GetEstablishmentsRequest request = new();

        // Act
        UseCaseResponse<IReadOnlyCollection<Establishment>> result =
            await sut.HandleRequestAsync(request, _token);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Equal("An unexpected error occurred while processing the request.", result.ErrorMessage);

        repoMock.Verify(repository => repository.GetEstablishments(_token), Times.Once);
        loggerMock.VerifyErrorContains("encountered an unexpected error");
    }
}


internal static class LoggerVerifyExtensions
{
    public static void VerifyNoErrors<T>(this Mock<ILogger<T>> mock)
    {
        mock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    public static void VerifyErrorContains<T>(
        this Mock<ILogger<T>> mock,
        string expected)
    {
        mock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains(expected)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
