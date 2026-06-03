using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model
{
    public sealed class EstablishmentExceptionTests
    {
        [Fact]
        public void Constructor_WithMessage_ShouldSetMessage()
        {
            // Arrange
            string message = "Something went wrong";

            // Act
            EstablishmentException ex = new(message);

            // Assert
            Assert.Equal(message, ex.Message);
            Assert.Null(ex.InnerException);
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
        {
            // Arrange
            string message = "Validation failed";
            Exception inner = new InvalidOperationException("Inner");

            // Act
            EstablishmentException ex = new(message, inner);

            // Assert
            Assert.Equal(message, ex.Message);
            Assert.Equal(inner, ex.InnerException);
        }

        [Fact]
        public void Exception_ShouldInheritFromApplicationException()
        {
            // Act
            EstablishmentException ex = new("Test");

            // Assert
            Assert.IsType<ApplicationException>(ex, exactMatch: false);
        }
    }
}
