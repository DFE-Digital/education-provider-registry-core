using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments;

public sealed class CompositionRootTests
{
    [Fact]
    public void AddEstablishmentsUseCaseDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddEstablishmentsUseCaseDependencies(null!));
    }

    [Fact]
    public void AddEstablishmentsInfrastructureDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddEstablishmentsInfrastructureDependencies(null!));
    }

    [Fact]
    public void AddEstablishmentsUseCaseDependencies_ShouldRegisterUseCase()
    {
        // Arrange
        IServiceCollection services = IServiceCollectionTestDoubles.Default();

        // Register dummy dependencies required by the use case
        services.AddSingleton<ILogger<GetEstablishmentsUseCase>, DummyLogger<GetEstablishmentsUseCase>>();
        services.AddSingleton<IEstablishmentsRepository, DummyRepository>();

        // Act
        IServiceCollection updated = services.AddEstablishmentsUseCaseDependencies();
        ServiceProvider provider = updated.BuildServiceProvider();

        // Assert
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<Establishment>>> useCase =
            provider.GetService<IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<Establishment>>>>();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        Assert.NotNull(useCase);
        Assert.IsType<GetEstablishmentsUseCase>(useCase);
    }


    [Fact]
    public void AddEstablishmentsInfrastructureDependencies_ShouldRegisterRepositoryAndMappers()
    {
        // Arrange
        IServiceCollection services = IServiceCollectionTestDoubles.Default();

        // Act
        IServiceCollection updated = services.AddEstablishmentsInfrastructureDependencies();
        ServiceProvider provider = updated.BuildServiceProvider();

        // Assert — Repository
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        IEstablishmentsRepository repository =
            provider.GetService<IEstablishmentsRepository>();

        Assert.NotNull(repository);
        Assert.IsType<EstablishmentsRepository>(repository);

        // Assert — Single-item mapper
        IMapper<EstablishmentDataTransferObject, Establishment> singleMapper =
            provider.GetService<IMapper<EstablishmentDataTransferObject, Establishment>>();
        Assert.NotNull(singleMapper);
        Assert.IsType<EstablishmentDtoToModelMapper>(singleMapper);

        // Assert — Collection mapper
        IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>> collectionMapper =
            provider.GetService<IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>>();
        Assert.NotNull(collectionMapper);
        Assert.IsType<EstablishmentsDtoToModelMapper>(collectionMapper);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
    }

    [Fact]
    public void AddEstablishmentsInfrastructureDependencies_ShouldRegisterCorrectLifetimes()
    {
        // Arrange
        IServiceCollection services = IServiceCollectionTestDoubles.Default();

        // Act
        IServiceCollection updated = services.AddEstablishmentsInfrastructureDependencies();

        // Assert lifetimes
        ServiceDescriptor repoDescriptor =
            Assert.Single(updated, serviceDesc =>
                serviceDesc.ServiceType == typeof(IEstablishmentsRepository));
        Assert.Equal(ServiceLifetime.Scoped, repoDescriptor.Lifetime);

        ServiceDescriptor singleMapperDescriptor =
            Assert.Single(updated, serviceDesc =>
                serviceDesc.ServiceType == typeof(IMapper<EstablishmentDataTransferObject, Establishment>));
        Assert.Equal(ServiceLifetime.Singleton, singleMapperDescriptor.Lifetime);

        ServiceDescriptor collectionMapperDescriptor =
            Assert.Single(updated, serviceDesc =>
            serviceDesc.ServiceType == typeof(IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>));
        Assert.Equal(ServiceLifetime.Singleton, collectionMapperDescriptor.Lifetime);
    }

    [Fact]
    public void CompositionRoot_ShouldResolveFullGraph()
    {
        // Arrange
        IServiceCollection services = IServiceCollectionTestDoubles.Default();

        // Register dummy dependencies required by the use case
        services.AddSingleton<ILogger<GetEstablishmentsUseCase>, DummyLogger<GetEstablishmentsUseCase>>();
        services.AddSingleton<IEstablishmentsRepository, DummyRepository>();

        // Register module dependencies
        services.AddEstablishmentsUseCaseDependencies();
        services.AddEstablishmentsInfrastructureDependencies();

        ServiceProvider provider = services.BuildServiceProvider();

        // Act
#pragma warning disable CS8600
        IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<Establishment>>> useCase =
            provider.GetService<IUseCase<GetEstablishmentsRequest, UseCaseResponse<IReadOnlyCollection<Establishment>>>>();
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
