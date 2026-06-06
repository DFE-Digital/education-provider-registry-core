using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Tests.Shared.Services;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments;

public sealed class CompositionRootAddInfrastructureTests
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
        IServiceCollection services = ServiceCollectionDefault.Create();

        // Act
        IServiceCollection updated = services.AddEstablishmentsInfrastructureDependencies();

        // Assert lifetimes
        updated.ShouldContain<IEstablishmentsRepository, EstablishmentsRepository>(ServiceLifetime.Scoped);
        updated.ShouldContain<IMapper<EstablishmentDataTransferObject, Establishment>, EstablishmentDtoToModelMapper>(ServiceLifetime.Singleton);
        updated.ShouldContain<IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>, EstablishmentsDtoToModelMapper>(ServiceLifetime.Singleton);
    }
}
