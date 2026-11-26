using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;
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
                GetGroupByGroupUidRequest,
                UseCaseResponse<GroupReadModelResponse>>,
            GetGroupByGroupUidUseCase>();

        services.TryAddSingleton<
            IMapper<
                Group,
                GroupReadModel>, GroupToGroupReadModelMapper>();

        services.TryAddSingleton<
            IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberReadModel>>, MemberToMemberReadModelMapper>();

        services.TryAddSingleton<
            IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeReadModel>>, TrusteeToTrusteeReadModelMapper>();

        services.TryAddSingleton<
            IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>>, EstablishmentsToEstablishmentReadModelMapper>();
        return services;
    }

    public static IServiceCollection AddGroupsInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .TryAddScoped<IGroupsRepository, GroupsRepository>();

        services.TryAddSingleton<
            IMapper<
                GroupMappingContext,
                Group>, GroupMappingContextToGroupMapper>();

        services.TryAddSingleton<
            IMapper<
                IEnumerable<GroupEstablishmentAggregate>,
                IReadOnlyCollection<Establishment>>,
            GroupEstablishmentAggregateToEstablishmentsMapper>();

        services.TryAddSingleton<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Trustee>>,
            GroupGovernorAggregateToTrusteesMapper>();

        services.TryAddSingleton<
            IMapper<
                IEnumerable<GroupGovernorAggregate>,
                IReadOnlyCollection<Member>>,
            GroupGovernorAggregateToMembersMapper>();

        return services;
    }
}
