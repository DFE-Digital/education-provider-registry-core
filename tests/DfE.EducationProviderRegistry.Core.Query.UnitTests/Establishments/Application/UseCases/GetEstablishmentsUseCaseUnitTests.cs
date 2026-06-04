using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.UseCases;

public sealed class GetEstablishmentsUseCaseUnitTests
{
    private readonly CancellationToken _token = CancellationToken.None;

    private static GetEstablishmentsUseCase CreateSut(
        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock,
        Mock<IEstablishmentsRepository> repoMock) => new(loggerMock.Object, repoMock.Object);

    [Fact]
    public async Task HandleRequestAsync_ReturnsMappedEstablishments()
    {
        // Arrange
        IReadOnlyCollection<Establishment> establishmentResults =
            new EstablishmentCollectionBuilder()
                .WithCount(2)
                .Build();

        Mock<IEstablishmentsRepository> repoMock =
            MockTestDouble.For<IEstablishmentsRepository, IReadOnlyCollection<Establishment>>(
                (repo) => repo.GetEstablishments(_token), establishmentResults);

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            MockTestDouble.Default<ILogger<GetEstablishmentsUseCase>>();

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
            MockTestDouble.For<
                IEstablishmentsRepository, IReadOnlyCollection<Establishment>>(
                    (repo) => repo.GetEstablishments(_token),
                    null!);

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            MockTestDouble.Default<ILogger<GetEstablishmentsUseCase>>();

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
            MockTestDouble.ThrowsExceptionFor<IEstablishmentsRepository, IReadOnlyCollection<Establishment>, OperationCanceledException>(
                (repo) => repo.GetEstablishments(_token));

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            MockTestDouble.Default<ILogger<GetEstablishmentsUseCase>>();

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
            MockTestDouble.ThrowsExceptionFor<IEstablishmentsRepository, EstablishmentException>(
                repo => repo.GetEstablishments(_token),
                new EstablishmentException("establishment exception"));

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            MockTestDouble.Default<ILogger<GetEstablishmentsUseCase>>();

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
            MockTestDouble.ThrowsExceptionFor<IEstablishmentsRepository, IReadOnlyCollection<Establishment>, Exception>(
                repo => repo.GetEstablishments(_token));

        Mock<ILogger<GetEstablishmentsUseCase>> loggerMock =
            MockTestDouble.Default<ILogger<GetEstablishmentsUseCase>>();

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
