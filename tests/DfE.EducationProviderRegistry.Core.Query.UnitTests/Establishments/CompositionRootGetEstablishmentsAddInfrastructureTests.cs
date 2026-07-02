using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments;

public sealed class CompositionRootGetEstablishmentsAddInfrastructureTests
{
    [Fact]
    public void AddEstablishmentsInfrastructureDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddEstablishmentsInfrastructureDependencies(null!));
    }

    [Fact]
    public void AddEstablishmentsInfrastructureDependencies_ShouldRegisterCorrectDependencyDescriptors()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefaults.Create();

        // Act
        IServiceCollection updated = services.AddEstablishmentsInfrastructureDependencies();

        // Assert lifetimes
        updated.ShouldContain<IEstablishmentsRepository, EfPostgresEstablishmensRepository>(ServiceLifetime.Scoped);
        updated.ShouldContain<IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentDetailsModel>>, EstablishmentsToDetailsModelMapper>(ServiceLifetime.Singleton);
    }
}
