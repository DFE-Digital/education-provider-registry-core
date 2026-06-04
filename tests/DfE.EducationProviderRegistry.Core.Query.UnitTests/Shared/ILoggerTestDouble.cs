using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;

internal static class ILoggerTestDouble
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "<Pending>")]
    internal static Mock<ILogger<TLogCategory>> Mock<TLogCategory>(string expectedMessage)
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

internal sealed class DummyLogger<TLogInstance> : ILogger<TLogInstance>
{
    public static readonly DummyScope Instance = new();
#pragma warning disable CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
    public IDisposable BeginScope<TState>(TState state) => Instance;
#pragma warning restore CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.

    public bool IsEnabled(LogLevel logLevel) => false;

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public void Log<TState>(
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        // no-op
    }

    internal sealed class DummyScope : IDisposable
    {
        public void Dispose() { }
    }
}

