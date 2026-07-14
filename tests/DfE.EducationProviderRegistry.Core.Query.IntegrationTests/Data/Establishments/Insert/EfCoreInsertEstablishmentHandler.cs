using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using static DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.EstablishmentBuilder;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Insert;

internal sealed class EfCoreInsertEstablishmentHandler : IInsertEstablishmentHandler
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public EfCoreInsertEstablishmentHandler(
        EducationProviderRegistryDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task InsertAsync(
        IReadOnlyCollection<Establishment> establishments,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(establishments);

        if (establishments.Count == 0)
        {
            return;
        }

        // Resolve reference data once
        EstablishmentReferenceData referenceData = new();

        long establishmentTypeId =
            await GetEstablishmentTypeIdAsync(
                _dbContext,
                referenceData.EstablishmentTypeCode);

        long establishmentStatusId =
            await GetEstablishmentStatusIdAsync(
                _dbContext,
                referenceData.EstablishmentStatusCode);

        long roleTypeId =
            await GetRoleTypeIdAsync(
                _dbContext,
                referenceData.HeadteacherRoleTypeCode);

        List<(Establishment Establishment, RoleAssignment RoleAssignment)> circularReferences = [];

        // Configure all graphs before first save
        foreach (Establishment establishment in establishments)
        {
            establishment.EstablishmentTypeId =
                establishmentTypeId;

            establishment.EstablishmentStatusId =
                establishmentStatusId;

            RoleAssignment roleAssignment =
                establishment.RoleAssignment.Single();

            roleAssignment.Role.RoleTypeId =
                roleTypeId;

            circularReferences.Add(
                (establishment, roleAssignment));

            // Break circular FK
            establishment.HeadteacherRoleAssignment = null;
            establishment.HeadteacherRoleAssignmentId = null;
        }

        // Insert everything in one batch
        _dbContext.Establishment.AddRange(establishments);
        await _dbContext.SaveChangesAsync(ct);

        // Reconnect circular FK once identities exist
        foreach (
            (Establishment establishment,
            RoleAssignment roleAssignment)
            in circularReferences)
        {
            establishment.HeadteacherRoleAssignmentId =
                roleAssignment.RoleAssignmentId;
        }

        // Persist FK updates
        await _dbContext.SaveChangesAsync(ct);
    }

    private static Task<long> GetEstablishmentTypeIdAsync(
        EducationProviderRegistryDbContext dbContext,
        string code)
    {
        return dbContext.EstablishmentType
            .Where(x => x.Code == code)
            .Select(x => x.EstablishmentTypeId)
            .SingleAsync();
    }

    private static Task<long> GetEstablishmentStatusIdAsync(
        EducationProviderRegistryDbContext dbContext,
        string code)
    {
        return dbContext.EstablishmentStatus
            .Where(x => x.Code == code)
            .Select(x => x.EstablishmentStatusId)
            .SingleAsync();
    }

    private static Task<long> GetRoleTypeIdAsync(
        EducationProviderRegistryDbContext dbContext,
        string code)
    {
        return dbContext.RoleType
            .Where(x => x.Code == code)
            .Select(x => x.RoleTypeId)
            .SingleAsync();
    }
}
