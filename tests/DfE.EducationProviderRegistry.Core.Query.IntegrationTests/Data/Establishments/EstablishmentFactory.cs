using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Builders;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;

internal sealed class EstablishmentFactory : IEstablishmentFactory
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public EstablishmentFactory(
        EducationProviderRegistryDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task<CreatedEstablishmentResult> CreateAsync(
        Action<EstablishmentBuilder>? configure = null)
    {
        // Arrange
        EstablishmentBuilder builder = new();

        configure?.Invoke(builder);

        Establishment establishment = builder.Build();

        establishment.EstablishmentTypeId =
            await GetEstablishmentTypeIdAsync(
                _dbContext,
                builder.ReferenceData.EstablishmentTypeCode);

        establishment.EstablishmentStatusId =
            await GetEstablishmentStatusIdAsync(
                _dbContext,
                builder.ReferenceData.EstablishmentStatusCode);

        RoleAssignment roleAssignment =
            establishment.RoleAssignment.Single();

        roleAssignment.Role.RoleTypeId =
            await GetRoleTypeIdAsync(
                _dbContext,
                builder.ReferenceData.HeadteacherRoleTypeCode);

        // IMPORTANT:
        // Break the circular dependency before first save.
        establishment.HeadteacherRoleAssignment = null;
        establishment.HeadteacherRoleAssignmentId = null;

        // Persist graph
        _dbContext.Establishment.Add(establishment);

        await _dbContext.SaveChangesAsync();

        // Reconnect circular FK once identities exist
        establishment.HeadteacherRoleAssignmentId =
            roleAssignment.RoleAssignmentId;

        await _dbContext.SaveChangesAsync();

        return new(establishment.EstablishmentId);
    }

    private static async Task<long> GetEstablishmentTypeIdAsync(
        EducationProviderRegistryDbContext context,
        string code)
    {
        return await context.EstablishmentType
            .Where(x => x.Code == code)
            .Select(x => x.EstablishmentTypeId)
            .SingleAsync();
    }

    private static async Task<long> GetEstablishmentStatusIdAsync(
        EducationProviderRegistryDbContext context,
        string code)
    {
        return await context.EstablishmentStatus
            .Where(x => x.Code == code)
            .Select(x => x.EstablishmentStatusId)
            .SingleAsync();
    }

    private static async Task<long> GetRoleTypeIdAsync(
        EducationProviderRegistryDbContext context,
        string code)
    {
        return await context.RoleType
            .Where(x => x.Code == code)
            .Select(x => x.RoleTypeId)
            .SingleAsync();
    }
}
