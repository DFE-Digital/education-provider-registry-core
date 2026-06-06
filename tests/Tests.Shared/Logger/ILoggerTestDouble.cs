using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Shared.Logger;

public static class ILoggerTestDouble
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "<Pending>")]
    public static Mock<ILogger<TLogCategory>> Mock<TLogCategory>(string expectedMessage)
    {
        return MockTestDouble.For<ILogger<TLogCategory>>(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }
}
