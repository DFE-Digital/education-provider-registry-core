using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal sealed class TestDbContextFactory : IDbContextFactory<EducationProviderRegistryDbContext>
{
    public static IDbContextFactory<EducationProviderRegistryDbContext> CreateFactory(EducationProviderRegistryDbContext context)
    {
        return new TestDbContextFactory(context);
    }

    private readonly EducationProviderRegistryDbContext _context;

    public TestDbContextFactory(EducationProviderRegistryDbContext context)
    {
        _context = context;
    }

    public EducationProviderRegistryDbContext CreateDbContext()
    {
        return _context;
    }

    public Task<EducationProviderRegistryDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_context);
    }
}
