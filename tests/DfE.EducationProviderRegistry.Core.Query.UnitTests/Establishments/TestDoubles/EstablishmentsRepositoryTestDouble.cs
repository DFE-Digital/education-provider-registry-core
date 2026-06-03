using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles;

internal static class EstablishmentsRepositoryTestDouble
{
    internal static Mock<IEstablishmentsRepository> Default() => new();

    internal static Mock<IEstablishmentsRepository> MockGetEstablishments(
        IReadOnlyCollection<Establishment> stub,
        CancellationToken cancellationToken)
    {
        Mock<IEstablishmentsRepository> mock = Default();

        mock.Setup(
            repo => repo.GetEstablishments(cancellationToken))
                .ReturnsAsync(stub)
                .Verifiable();

        return mock;
    }

    internal static Mock<IEstablishmentsRepository> MockGetEstablishmentsThrowsOperationCanceled(
        CancellationToken cancellationToken) =>
            MockGetEstablishmentsThrowsExceptionOfType<OperationCanceledException>(
                new OperationCanceledException(), cancellationToken);

    internal static Mock<IEstablishmentsRepository> MockGetEstablishmentsThrowsEstablishmentException(
        CancellationToken cancellationToken) =>
            MockGetEstablishmentsThrowsExceptionOfType<EstablishmentException>(
                new EstablishmentException("establishment exception"), cancellationToken);

    internal static Mock<IEstablishmentsRepository> MockGetEstablishmentsThrowsException(
        CancellationToken cancellationToken) =>
            MockGetEstablishmentsThrowsExceptionOfType<Exception>(
                new Exception("exception"), cancellationToken);

    private static Mock<IEstablishmentsRepository> MockGetEstablishmentsThrowsExceptionOfType<TException>(
        TException exception,
        CancellationToken cancellationToken) where TException : Exception
    {
        Mock<IEstablishmentsRepository> mock = Default();

        mock.Setup(
            repo => repo.GetEstablishments(cancellationToken))
                .ThrowsAsync(exception)
                .Verifiable();

        return mock;
    }
}