using System.Diagnostics;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Builders;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using static DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Builders.EstablishmentBuilder;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data;

internal sealed class EstablishmentFactory : IEstablishmentFactory
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public EstablishmentFactory(EducationProviderRegistryDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async Task<Establishment> CreateAsync(
        Action<EstablishmentBuilder>? configure = null,
        CancellationToken ct = default)
    {
        EstablishmentBuilder builder = new();

        configure?.Invoke(builder);

        Establishment establishment = builder.Build();

        await InsertEstablishmentsAsync(
            [establishment],
            ct);

        return establishment;
    }


    public async Task InsertEstablishmentsAsync(
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
            await _dbContext.GetEstablishmentTypeIdAsync(referenceData.EstablishmentTypeCode);

        long establishmentStatusId =
            await _dbContext.GetEstablishmentStatusIdAsync(referenceData.EstablishmentStatusCode);

        long roleTypeId =
            await _dbContext.GetRoleTypeIdAsync(referenceData.HeadteacherRoleTypeCode);

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

        Stopwatch watch = Stopwatch.StartNew();

        // Insert everything in one batch
        await _dbContext.BulkInsertAsync(
            establishments,
            bulkConfig: new BulkConfig()
            {
                IncludeGraph = true,
                SetOutputIdentity = true
            },
            cancellationToken: ct);

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
        await _dbContext.BulkUpdateAsync(
            establishments,
            bulkConfig: new BulkConfig()
            {
                SetOutputIdentity = true,
                IncludeGraph = true
            },
            cancellationToken: ct);
    }
}
