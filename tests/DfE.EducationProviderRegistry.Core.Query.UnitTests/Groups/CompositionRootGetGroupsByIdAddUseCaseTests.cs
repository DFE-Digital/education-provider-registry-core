using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Tests.Shared.Services;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups;

public sealed class CompositionRootGetGroupsByIdAddUseCaseTests
{

    [Fact]
    public void AddGroupsUseCaseDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddGroupsUseCaseDependencies(null!));
    }

    [Fact]
    public void AddGroupsUseCaseDependencies_ShouldRegisterCorrectDependencyDescriptors()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefault.Create();

        // Act
        IServiceCollection updated = services.AddGroupsUseCaseDependencies();

        // Assert
        updated.ShouldContain<
            IUseCase<GetGroupByGroupIdRequest, UseCaseResponse<GroupReadModel>>,
            GetGroupByGroupIdUseCase>(ServiceLifetime.Scoped);

        updated.ShouldContain<IMapper<Group, GroupReadModel>, GroupToGroupReadModelMapper>(ServiceLifetime.Singleton);
        updated.ShouldContain<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>, TrusteeToTrusteeReadModelMapper>(ServiceLifetime.Singleton);
        updated.ShouldContain<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>, MemberToMemberReadModelMapper>(ServiceLifetime.Singleton);
    }
}
