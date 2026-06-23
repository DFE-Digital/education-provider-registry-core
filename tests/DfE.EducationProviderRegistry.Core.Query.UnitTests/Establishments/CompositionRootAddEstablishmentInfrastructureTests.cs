using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using Microsoft.Extensions.DependencyInjection;

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
        IServiceCollection services = ServiceCollectionDefaults.Create();

        // Act
        IServiceCollection updated = services.AddEstablishmentsInfrastructureDependencies();

        // Assert lifetimes
        updated.ShouldContain<IEstablishmentsRepository, FakeDataEstablishmentsRepository>(ServiceLifetime.Scoped);
        updated.ShouldContain<IMapper<EstablishmentDto, Establishment>, EstablishmentDtoToModelMapper>(ServiceLifetime.Singleton);
        updated.ShouldContain<IMapper<IEnumerable<EstablishmentDto>, IReadOnlyCollection<Establishment>>, EstablishmentsDtoToModelMapper>(ServiceLifetime.Singleton);
    }
}
