using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

public class EducationProviderRegistryDbContext : DbContext
{
    public DbSet<EfEstablishmentDto> Establishments { get; set; }

    public EducationProviderRegistryDbContext(DbContextOptions<EducationProviderRegistryDbContext> options)
        : base(options)
    {
    }
}
