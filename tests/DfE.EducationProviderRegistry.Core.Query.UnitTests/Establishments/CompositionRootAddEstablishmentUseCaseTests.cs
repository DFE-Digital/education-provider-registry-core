using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments;

public sealed class CompositionRootAddEstablishmentUseCaseUnitTests
{

    [Fact]
    public void AddEstablishmentsUseCaseDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddEstablishmentsUseCaseDependencies(null!));
    }

    [Fact]
    public void AddEstablishmentsUseCaseDependencies_ShouldRegisterCorrectDependencyDescriptors()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefaults.Create();

        // Act
        IServiceCollection updated = services.AddEstablishmentsUseCaseDependencies();

        // Assert
        updated.ShouldContain<
            IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<Establishment>>>,
            GetEstablishmentsUseCase>(ServiceLifetime.Scoped);
    }
}
