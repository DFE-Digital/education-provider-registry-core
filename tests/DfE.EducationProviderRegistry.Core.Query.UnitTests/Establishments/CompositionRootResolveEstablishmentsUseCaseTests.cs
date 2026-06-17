using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tests.Shared.Logger;
using Tests.Shared.Services;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments;

public sealed class CompositionRootResolveEstablishmentsUseCaseTests
{
    [Fact]
    public void CompositionRoot_ShouldResolveFullGraph()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefault.Create();

        // Register dummy dependencies required by the use case
        services.AddSingleton<ILogger<GetEstablishmentsUseCase>, InMemoryLogger<GetEstablishmentsUseCase>>();
        services.AddSingleton<IEstablishmentsRepository, DummyRepository>();

        // Register module dependencies
        services.AddEstablishmentsUseCaseDependencies();
        services.AddEstablishmentsInfrastructureDependencies();

        ServiceProvider provider = services.BuildServiceProvider(ServiceProviderOptionsDefaults.Default);
        using IServiceScope scope = provider.CreateScope();

        // Act Assert

#pragma warning disable CS8600
        IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<Establishment>>> useCase =
            scope.ServiceProvider.GetService<IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<Establishment>>>>();
#pragma warning restore CS8600

        // Assert
        Assert.NotNull(useCase);
        Assert.IsType<GetEstablishmentsUseCase>(useCase);
    }

    private sealed class DummyRepository : IEstablishmentsRepository
    {
        public Task<IReadOnlyCollection<Establishment>> GetEstablishments(
            CancellationToken cancellationToken = default) =>
                Task.FromResult<IReadOnlyCollection<Establishment>>([]);
    }
}
