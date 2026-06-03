using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles;

internal static class ILoggerTestDouble
{
    internal static Mock<ILogger<TLogCategory>> Default<TLogCategory>() => new();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "<Pending>")]
    internal static Mock<ILogger<TLogCategory>> Mock<TLogCategory>(string expectedMessage)
    {
        Mock<ILogger<TLogCategory>> mock = Default<TLogCategory>();

        mock.Setup(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Verifiable();

        return mock;
    }
}
