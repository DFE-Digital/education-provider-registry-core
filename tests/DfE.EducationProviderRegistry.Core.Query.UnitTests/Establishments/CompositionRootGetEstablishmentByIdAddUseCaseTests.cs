using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments;

public sealed class CompositionRootGetEstablishmentByIdAddUseCaseTests
{
    [Fact]
    public void AddGetEstablishmentByIdUseCaseDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddEstablishmentsUseCaseDependencies(null!));
    }

    [Fact]
    public void AddGetEstablishmentByIdUseCaseDependencies_ShouldRegisterCorrectDependencyDescriptors()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefaults.Create();

        // Act
        IServiceCollection updated = services.AddEstablishmentsUseCaseDependencies();

        // Assert
        updated.ShouldContain<
            IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsModel?>>,
            GetEstablishmentByIdUseCase>(ServiceLifetime.Scoped);
    }
}
