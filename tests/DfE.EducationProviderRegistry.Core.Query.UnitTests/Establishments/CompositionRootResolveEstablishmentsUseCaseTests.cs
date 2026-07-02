using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments;

public sealed class CompositionRootResolveEstablishmentsUseCaseTests
{
    [Fact]
    public void CompositionRoot_ShouldResolveFullGraph()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefaults.Create();

        // Register dummy dependencies required by the use case
        services.AddSingleton<ILogger<GetEstablishmentsUseCase>, InMemoryLogger<GetEstablishmentsUseCase>>();
        services.AddSingleton<ILogger<GetEstablishmentByIdUseCase>, InMemoryLogger<GetEstablishmentByIdUseCase>>();
        services.AddScoped<IEstablishmentsRepository, EfPostgresEstablishmensRepository>();

        services.AddDbContext<EducationProviderRegistryDbContext>(options =>
        {
            options.UseInMemoryDatabase("CompositionRootTest");
        });


        // Register module dependencies
        services.AddEstablishmentsUseCaseDependencies();
        services.AddEstablishmentsInfrastructureDependencies();

        ServiceProvider provider = services.BuildServiceProvider(ServiceProviderOptionsDefaults.Default);
        using IServiceScope scope = provider.CreateScope();

        // Act Assert
#pragma warning disable CS8600
        IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>> getEstablishmentsUseCase =
            scope.ServiceProvider.GetService<IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>>>>();
        IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsModel?>> getEstablishmentByIdUseCase =
            scope.ServiceProvider.GetService<IUseCase<GetEstablishmentByIdRequest, UseCaseResponse<EstablishmentDetailsModel?>>>();
#pragma warning restore CS8600

        // Assert
        Assert.NotNull(getEstablishmentsUseCase);
        Assert.NotNull(getEstablishmentByIdUseCase);
        Assert.IsType<GetEstablishmentsUseCase>(getEstablishmentsUseCase);
        Assert.IsType<GetEstablishmentByIdUseCase>(getEstablishmentByIdUseCase);
    }
}
