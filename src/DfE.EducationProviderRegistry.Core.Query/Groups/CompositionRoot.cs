using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Core.Query.Groups;

public static class CompositionRoot
{
    public static IServiceCollection AddGroupsUseCaseDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<
            IUseCase<
                GetGroupByGroupUniqueIdentifierRequest,
                UseCaseResponse<GroupReadModel>>,
            GetGroupByGroupIdUseCase>();

        services.TryAddSingleton<
            IMapper<Group, GroupReadModel>, GroupToGroupReadModelMapper>();

        services.TryAddSingleton<
            IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>, MemberToMemberReadModelMapper>();

        services.TryAddSingleton<
            IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>, TrusteeToTrusteeReadModelMapper>();

        return services;
    }

    public static IServiceCollection AddGroupsInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .TryAddScoped<IGroupsRepository, GroupsRepository>();

        services.TryAddSingleton<
            IMapper<GroupRecord, Group>, GroupRecordToGroupMapper>();

        return services;
    }
}
