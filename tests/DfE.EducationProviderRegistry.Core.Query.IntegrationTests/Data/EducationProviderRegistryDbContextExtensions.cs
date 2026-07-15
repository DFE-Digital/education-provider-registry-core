using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data;

internal static class EducationProviderRegistryDbContextExtensions
{
    internal static Task<long> GetEstablishmentTypeIdAsync(
        this EducationProviderRegistryDbContext dbContext,
        string code)
    {
        return dbContext.EstablishmentType
            .Where(x => x.Code == code)
            .Select(x => x.EstablishmentTypeId)
            .SingleAsync();
    }

    internal static Task<long> GetEstablishmentStatusIdAsync(
        this EducationProviderRegistryDbContext dbContext,
        string code)
    {
        return dbContext.EstablishmentStatus
            .Where(x => x.Code == code)
            .Select(x => x.EstablishmentStatusId)
            .SingleAsync();
    }

    internal static Task<long> GetRoleTypeIdAsync(
        this EducationProviderRegistryDbContext dbContext,
        string code)
    {
        return dbContext.RoleType
            .Where(x => x.Code == code)
            .Select(x => x.RoleTypeId)
            .SingleAsync();
    }
}
