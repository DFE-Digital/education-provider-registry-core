using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;

public interface IQueryEngine
{
    IQueryable<TProjection> BuildQuery<TEntity, TProjection>(
        ISpecification<TEntity> spec,
        Expression<Func<TEntity, TProjection>> projection)
        where TEntity : class;

    IQueryable<TProjection> BuildQuery<TEntity, TProjection>(
        Expression<Func<TEntity, TProjection>> projection)
        where TEntity : class;
}
