using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<EfEstablishmentDto> Establishments { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
