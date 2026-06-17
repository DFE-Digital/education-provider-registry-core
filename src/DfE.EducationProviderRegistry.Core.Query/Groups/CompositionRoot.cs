using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence;
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
                GetGroupByGroupIdRequest,
                UseCaseResponse<GroupDto>>,
            GetGroupByGroupIdUseCase>();

        services.TryAddSingleton<IMapper<Group, GroupDto>, GroupToGroupDtoMapper>();
        services.TryAddSingleton<IMapper<IEnumerable<Member>, IReadOnlyCollection<MemberDto>>, MemberToMemberDtoMapper>();
        services.TryAddSingleton<IMapper<IEnumerable<Trustee>, IReadOnlyCollection<TrusteeDto>>, TrusteeToTrusteeDtoMapper>();

        return services;
    }

    public static IServiceCollection AddGroupsInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .TryAddScoped<IGroupsRepository, FakeGroupsRepository>();

        return services;
    }
}
