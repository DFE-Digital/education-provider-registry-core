using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Shared.Logger;

public static class LoggerVerifyExtensions
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

    public static void VerifyErrorContains<T>(this Mock<ILogger<T>> mock, string expected)
        => mock.VerifyLogContains(LogLevel.Error, expected);

    public static void VerifyWarningContains<T>(this Mock<ILogger<T>> mock, string expected)
        => mock.VerifyLogContains(LogLevel.Warning, expected);

    private static void VerifyLogContains<T>(
        this Mock<ILogger<T>> mock,
        LogLevel level,
        string expected)
    {
        mock.Verify(
            logger => logger.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains(expected)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
