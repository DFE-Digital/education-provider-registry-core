using DfE.EducationProviderRegistry.Core.Query.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups;

public sealed class CompositionRootGetGroupsByIdAddInfrastructureTests
{
    [Fact]
    public void AddGroupsInfrastructureDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddGroupsInfrastructureDependencies(null!));
    }

    [Fact]
    public void AddGroupsInfrastructureDependencies_ShouldRegisterCorrectDependencyDescriptors()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefaults.Create();

        // Act
        IServiceCollection updated = services.AddGroupsInfrastructureDependencies();

        // Assert lifetimes
        updated.ShouldContain<IGroupsRepository, FakeGroupsRepository>(ServiceLifetime.Scoped);
    }
}
