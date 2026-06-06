using System.Linq.Expressions;
using Moq;

namespace Tests.Shared;

public static class MockTestDouble
{
    public static Mock<TMock> Default<TMock>() where TMock : class => new();

    public static Mock<TMock> For<TMock, TOutput>(Expression<Func<TMock, TOutput>> expression, TOutput response) where TMock : class
    {
        Mock<TMock> mock = Default<TMock>();
        mock.Setup(expression)
            .Returns(response)
            .Verifiable();

        return mock;
    }

    public static Mock<TMock> For<TMock, TOutput>(Expression<Func<TMock, Task<TOutput>>> expression, TOutput response) where TMock : class
    {
        Mock<TMock> mock = Default<TMock>();
        mock.Setup(expression)
            .ReturnsAsync(response)
            .Verifiable();

        return mock;
    }

    public static Mock<TMock> For<TMock>(Expression<Action<TMock>> expression) where TMock : class
    {
        Mock<TMock> mock = Default<TMock>();
        mock.Setup(expression)
            .Verifiable();
        return mock;
    }

    public static Mock<TMock> ThrowsExceptionFor<TMock, TOutput, TException>(Expression<Func<TMock, Task<TOutput>>> expression)
        where TMock : class
        where TException : Exception, new()
    {
        Mock<TMock> mock = Default<TMock>();
        mock.Setup(expression)
            .ThrowsAsync(new TException())
            .Verifiable();
        return mock;
    }

    public static Mock<TMock> ThrowsExceptionFor<TMock, TException>(Expression<Action<TMock>> expression, TException exception)
        where TMock : class
        where TException : Exception
    {
        Mock<TMock> mock = Default<TMock>();
        mock.Setup(expression)
            .Throws(exception)
            .Verifiable();
        return mock;
    }
}
