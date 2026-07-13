using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;

public interface ISearchProjectionBuilder<TProjection>
{
    IQueryable<TProjection> Build(DbContext db);
}
